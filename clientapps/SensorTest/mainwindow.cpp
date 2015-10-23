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
#include "configuration/facedetectionsettings/facedetectionsettingsmodel.h"


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_client(new QFruitHapClient(QString("FruitHAP_RpcExchange"), QString("FruitHAP_PubSubExchange"),this)),        
    m_configControl(m_client, new QOpenCvFaceVerifier(1,false,this), this),
    m_switchBoard(0),
    m_door(new Door(this))


{
    ui->setupUi(this);
    connect(&m_configControl,&QConfigurationControl::sensorListLoaded,this,&MainWindow::onSensorListLoaded);
    connect(m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);    
    m_statusBarLabel = new QLabel();
    ui->statusBar->addWidget(m_statusBarLabel);

    onDisconnected();
}

MainWindow::~MainWindow()
{    
    delete ui;

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

void MainWindow::onSensorListLoaded()
{        
    //loadFaceDetectionSettings();
    initDoorSetup();
    loadSwitchboard();
    loadCameraView();

    qDebug() << "Sensor list loaded..";
}

void MainWindow::initDoorSetup()
{

    connect(m_door,&Door::initialize,this,&MainWindow::onDoorInitialize);
    connect(m_door,&Door::personApproved,this, &MainWindow::onDoorAccessGranted);
    connect(m_door,&Door::personNotApproved,this, &MainWindow::onDoorAccessDenied);
    connect(m_door,&Door::noAnswer,this, &MainWindow::onDoorNoAnswer);

    QCamera* doorCamera = dynamic_cast<QCamera*>(m_configControl.getSensorByName(DOORCAMERA_NAME));

    if (doorCamera != nullptr)
    {
        doorCamera->enableFaceDetection(true);
        QCameraWidget *doorCameraWidget = new QCameraWidget(doorCamera->getName(),false, doorCamera->isFaceDetectionEnabled(),this);
        connect(m_door,&Door::imageWithFaceIsAvailable, doorCameraWidget, &QCameraWidget::onImageReceived);

    }
}

void MainWindow::loadFaceDetectionSettings()
{
    QList<FaceDetectionSetting> settings;
    FaceDetectionSettingsModel model;
    model.LoadSettingsFromFile("settings.json",settings);

    foreach (const FaceDetectionSetting &setting, settings)
    {
        if (setting.isFaceDetectionEnabled())
        {
            m_configControl.coupleFaceDetectionToSwitch(setting.getCameraName(),setting.getSwitchName());
        }
    }
}

void MainWindow::loadCameraView()
{
   QGridLayout *layout = new QGridLayout();
   layout->setMargin(0);
   QWidget *widget = new QWidget(this);
   QList<QCamera*> cameraList;

   m_configControl.getAllCameras(cameraList);

   int col = 0;
   int row = 0;
   foreach (QCamera *aCamera, cameraList)
   {
       QCameraWidget *cameraWidget = new QCameraWidget(aCamera->getName(),aCamera->isPollable(), aCamera->isFaceDetectionEnabled());
       connect(cameraWidget,&QCameraWidget::refresh,aCamera, &QCamera::getValue);
       connect(aCamera,&QCamera::imageReceived, cameraWidget, &QCameraWidget::onImageReceived);
       connect(aCamera,&QCamera::faceDetected, this, &MainWindow::onFaceDetected);
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
   }

   layout->setAlignment(layout,Qt::AlignTop);
   widget->setLayout(layout);
   ui->cameraList->setWidget(widget);
}


void MainWindow::loadSwitchboard()
{
    QList<QSwitch*> switchList;
    QWidget *switchBoardWidget = new QWidget(this);
    QVBoxLayout *layout = new QVBoxLayout();

    m_configControl.getAllSwitches(switchList);

    foreach (QSwitch *aSwitch, switchList)
    {
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

    switchBoardWidget->setLayout(layout);
    ui->switchPage->setWidget(switchBoardWidget);


}

void MainWindow::onDisconnected()
{
    ui->actionConnect->setEnabled(true);
    ui->actionDisconnect->setEnabled(false);
    ui->statusBar->setStyleSheet("background-color: red");
    m_statusBarLabel->setText("Not connected..");
    m_statusBarLabel->setStyleSheet("color: white");
}

void MainWindow::onFaceDetected(const QString name, const QByteArray imageData, const QDateTime timestamp)
{
    ui->statusBar->setStyleSheet("background-color: red");
    m_statusBarLabel->setText("Oh oh.. person detected on camera " + name + "@ " + timestamp.toString());
    m_statusBarLabel->setStyleSheet("color: white");

    if (name == DOORCAMERA_NAME)
    {
        m_door->faceHasBeenDetected(imageData);
    }

}






void MainWindow::onErrorReceived(const QString name, const QString errorMessage)
{
    qCritical() << "Error received from " << name << ". Message: " << errorMessage;
}




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
    m_configControl.requestSensorList();
}
