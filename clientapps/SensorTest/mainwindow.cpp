#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "sensor/camera/qcamera.h"
#include "sensor/switch/qswitch.h"
#include <QInputDialog>
#include <QPixmap>
#include <widgets/qswitchwidget.h>
#include <QLayout>
#include <QGraphicsView>
#include <QGraphicsTextItem>
#include <QThread>
#include <widgets/qcamerawidget.h>
#include "faceverifier/qopencvfaceverifier.h"


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),    
    m_client(new QFruitHapClient(QString("FruitHAP_RpcExchange"), QString("FruitHAP_PubSubExchange"),this)),    
    m_configControl(m_client,this),    
    m_eventedSensors(),
    m_switchBoard(0)


{
    ui->setupUi(this);
    QOpenCvFaceVerifier *bla = new QOpenCvFaceVerifier(1,false,this);
    m_faceVerifier = bla;
    connect(&m_configControl,&QConfigurationControl::sensorListReceived,this,&MainWindow::onSensorListReceived);
    connect(m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);    
    m_statusBarLabel = new QLabel();
    ui->statusBar->addWidget(m_statusBarLabel);

    onDisconnected();
}

MainWindow::~MainWindow()
{    
    delete ui;
    qDeleteAll(m_eventedSensors);
}

QString convertEnumToString(const SwitchState& state )
{
    switch (state)
    {
    case SwitchState::Undefined:
        return QString("Undefined");
    case SwitchState::On:
        return QString("On");
    case SwitchState::Off:
        return QString("Off");
    }

    return "Undefined";
}




void MainWindow::connectToMQ(const QStringList &bindingKeys, const QString &uri)
{
    m_client->setPubSubTopics(bindingKeys);
    bool isConnected = m_client->connectToServer(uri);
    if (!isConnected)
    {
        QString uristring = !uri.isEmpty() ? uri : "localhost";
        qCritical() << "Cannot connect to: " << uristring;
        onDisconnected();

    }

}

void MainWindow::onConnected(const QString uri)
{
    ui->actionConnect->setEnabled(false);
    ui->actionDisconnect->setEnabled(true);

    loadSensors();
    ui->statusBar->setStyleSheet("background-color: green");
    m_statusBarLabel->setText("Connected to "+uri);
    m_statusBarLabel->setStyleSheet("color: white");

}

void MainWindow::onSensorListReceived(const QList<SensorData> list)
{
    m_eventedSensors.clear();
    foreach (SensorData item, list)
    {
        if (item.getCategory() == "Switch")
        {            
            QSwitch *eventedSwitch = new QSwitch(m_client,item.getName(),true,item.IsReadOnly(),parent());

            connect(eventedSwitch, &QSwitch::errorEventReceived, this, &MainWindow::onErrorReceived);            
            m_eventedSensors.append(eventedSwitch);
        }


        if( (item.getType() == "ButtonWithCameraSensor") || (item.getType() == "SwitchWithCameraSensor"))
        {
            QCamera *eventedCamera = new QCamera(m_client,item.getName(),false,item.IsReadOnly(),m_faceVerifier,parent());
            m_eventedSensors.append(eventedCamera);
        }

        if( (item.getType() == "Camera"))
        {
            QCamera *camera = new QCamera(m_client,item.getName(),true,item.IsReadOnly(),m_faceVerifier,parent());


            m_eventedSensors.append(camera);
        }

    }

    QString faceDetectionSwitchName = "RedLight";       //Ugly and hardcoded but will do for demo purposes
    QSwitch *faceDetectionSwitch = dynamic_cast<QSwitch*>(getSensorByName(faceDetectionSwitchName));

    foreach (QFruitHapSensor *sensor, m_eventedSensors)
    {
        QCamera* camera = dynamic_cast<QCamera*>(sensor);
        if (camera != nullptr)
        {
            camera->enableFaceDetection(true);
            connect(camera,&QCamera::faceDetected, faceDetectionSwitch, &QSwitch::turnOn);
            connect(camera,&QCamera::faceDetected, this, &MainWindow::onFaceDetected);
        }
    }

    loadSwitchboard();
    loadCameraView();
}

QFruitHapSensor* MainWindow::getSensorByName(const QString &name) const
{
    foreach (QFruitHapSensor* item, m_eventedSensors)
    {
        if (item->getName() == name)
        {
            return item;
        }
    }

    return nullptr;
}

void MainWindow::loadCameraView()
{
   QGridLayout *layout = new QGridLayout();
   layout->setMargin(0);
   QWidget *widget = new QWidget(this);
   int col = 0;
   int row = 0;
   foreach (QFruitHapSensor *sensor, m_eventedSensors)
   {
       QCamera *aCamera = dynamic_cast<QCamera*>(sensor);

       if (aCamera != nullptr)
       {
           QCameraWidget *cameraWidget = new QCameraWidget(aCamera->getName(),aCamera->isPollable());
           connect(cameraWidget,&QCameraWidget::refresh,aCamera, &QCamera::getValue);
           connect(aCamera,&QCamera::imageReceived, cameraWidget, &QCameraWidget::onImageReceived);

           layout->addWidget(cameraWidget,row,col,1,1);
           if (row >= 1)
           {
               col++;
               row = 0;
           }
           else
           {
               row++;
           }

           //           QGraphicsView* gr = new QGraphicsView();

//           gr->setMinimumWidth(640);
//           gr->setMaximumWidth(640);
//           gr->setMinimumHeight(480);
//           gr->setMaximumHeight(480);



//           QGraphicsScene *drawing = new QGraphicsScene(this);
//           QPixmap pixmap = QPixmap::fromImage(QImage("/home/maarten/home-cat.jpg"));//.scaled(drawing->width(),drawing->height());

//           drawing->addPixmap(pixmap.scaled(gr->width() - 5,gr->height() - 5));
//           drawing->addText(QString(aCamera->getName()));
//           gr->setScene(drawing);
//           layout->addWidget(gr);
       }
   }

   //widget->setGeometry(0,0,700,700);
   layout->setAlignment(layout,Qt::AlignTop);
   widget->setLayout(layout);
   ui->cameraList->setWidget(widget);
   //ui->tab_2->setLayout(layout);
}

void MainWindow::loadSwitchboard()
{
    if (m_switchBoard != nullptr)
    {
        delete m_switchBoard;
        m_switchBoard = nullptr;
    }

    m_switchBoard = new QWidget(this);    

    QVBoxLayout *layout = new QVBoxLayout();
    QTimer *timer = new QTimer(this);
    timer->setSingleShot(true);
    foreach (QFruitHapSensor *sensor, m_eventedSensors)
    {
        QSwitch *aSwitch = dynamic_cast<QSwitch*>(sensor);

        if (aSwitch != nullptr)
        {
            QSwitchWidget *widget = new QSwitchWidget(aSwitch->getName(),aSwitch->isReadOnly(), aSwitch->isPollable());

            connect(widget,&QSwitchWidget::turnOn,aSwitch, &QSwitch::turnOn);
            connect(widget,&QSwitchWidget::turnOff,aSwitch, &QSwitch::turnOff);
            connect(widget,&QSwitchWidget::refresh,aSwitch, &QSwitch::getValue);
            connect(aSwitch,&QSwitch::switchStateReceived, widget, &QSwitchWidget::onStateChanged);
            layout->addWidget(widget);
        }
    }

    m_switchBoard->setLayout(layout);
    ui->switchPage->setWidget(m_switchBoard);


}

void MainWindow::onDisconnected()
{
    ui->actionConnect->setEnabled(true);
    ui->actionDisconnect->setEnabled(false);
    ui->statusBar->setStyleSheet("background-color: red");
    m_statusBarLabel->setText("Not connected..");
    m_statusBarLabel->setStyleSheet("color: white");
}

void MainWindow::onFaceDetected(const QString name, const QDateTime timestamp)
{
    ui->statusBar->setStyleSheet("background-color: red");
    m_statusBarLabel->setText("Oh oh.. person detected on camera " + name + "@ " + timestamp.toString());
    m_statusBarLabel->setStyleSheet("color: white");
}






void MainWindow::onErrorReceived(const QString name, const QString errorMessage)
{
    qCritical() << "Error received from " << name << ". Message: " << errorMessage;
}


//void MainWindow::onImageDataReceived(const QString name, const QByteArray imageData)
//{
//    if (m_selectedSensor != nullptr && m_selectedSensor->getName() == name)
//    {
//        QImage image = QImage::fromData(imageData);
//        if (m_drawing != nullptr)
//        {
//            delete m_drawing;
//            m_drawing = nullptr;
//        }

//        m_drawing = new QGraphicsScene(this);
//        m_drawing->addPixmap(QPixmap::fromImage(image));
//        m_drawing->setSceneRect(image.rect());

//        ui->cameraImage->setScene(m_drawing);
//    }
//}


void MainWindow::on_actionConnect_triggered()
{
    bool ok;
    m_uri = QInputDialog::getText(this,"Connect to MQ server",
                                            "Enter AMQ connection string (default is amqp://guest:guest@localhost)", QLineEdit::Normal,
                                            "", &ok);
    if (ok)
    {
            QStringList bindingKeys;
            bindingKeys.push_back("alerts");

            connectToMQ(bindingKeys, m_uri);

    }

}

void MainWindow::on_actionDisconnect_triggered()
{
    m_client->disconnectFromServer();
}


void MainWindow::loadSensors()
{    
    m_configControl.getSensorList();
}
