#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "sensor/camera/qcamera.h"
#include "sensor/switch/qswitch.h"
#include <QInputDialog>
#include <QPixmap>
#include <widgets/qswitchwidget.h>
#include <QLayout>

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_drawing(nullptr),
    m_client(new QFruitHapClient(QString("FruitHAP_RpcExchange"), QString("FruitHAP_PubSubExchange"),this)),
    m_configControl(m_client,this),
    m_eventedSensors(),
    m_switchBoard(0)


{
    ui->setupUi(this);







    connect(&m_configControl,&QConfigurationControl::sensorListReceived,this,&MainWindow::onSensorListReceived);
    connect(m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);
    m_timer = new QTimer(this);
    m_statusBarLabel = new QLabel();
    ui->statusBar->addWidget(m_statusBarLabel);
    //connect(m_timer,&QTimer::timeout, this, &MainWindow::updateImage);
    onDisconnected();
}

MainWindow::~MainWindow()
{
    delete m_timer;
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

void MainWindow::onConnected()
{
    ui->actionConnect->setEnabled(false);
    ui->actionDisconnect->setEnabled(true);
    ui->dialRefreshrate->setEnabled(true);
    loadSensors();
    ui->statusBar->setStyleSheet("background-color: green");
    m_statusBarLabel->setText("Connected to "+m_uri);
    m_statusBarLabel->setStyleSheet("color: white");

}

void MainWindow::onSensorListReceived(const QList<SensorData> list)
{

    ui->cmbSensorList->clear();

    foreach (SensorData item, list)
    {
        if (item.getCategory() == "Switch")
        {            
            QSwitch *eventedSwitch = new QSwitch(m_client,item.getName(),true,item.IsReadOnly(),parent());
            connect(eventedSwitch, &QSwitch::switchStateReceived, this, &MainWindow::onSwitchStateReceived);
            connect(eventedSwitch, &QSwitch::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedSwitch);
        }


        if( (item.getType() == "ButtonWithCameraSensor") || (item.getType() == "SwitchWithCameraSensor"))
        {

            QCamera *eventedCamera = new QCamera(m_client,item.getName(),false,item.IsReadOnly(),parent());
            connect(eventedCamera, &QCamera::imageReceived, this, &MainWindow::onImageDataReceived);
            connect(eventedCamera, &QCamera::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedCamera);
        }

        if( (item.getType() == "Camera"))
        {
            QCamera *eventedCamera = new QCamera(m_client,item.getName(),true,item.IsReadOnly(),parent());
            connect(eventedCamera, &QCamera::imageReceived, this, &MainWindow::onImageDataReceived);
            connect(eventedCamera, &QCamera::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedCamera);
        }

        ui->cmbSensorList->addItem(item.getName());

    }

    loadSwitchboard();



}

void MainWindow::loadSwitchboard()
{
    if (m_switchBoard != nullptr)
    {
        delete m_switchBoard;
        m_switchBoard = nullptr;
    }

    m_switchBoard = new QWidget(this);
    m_switchBoard->setLayout(new QVBoxLayout(m_switchBoard));

    foreach (QFruitHapSensor *sensor, m_eventedSensors)
    {
        QSwitch *aSwitch = dynamic_cast<QSwitch*>(sensor);

        if (aSwitch != nullptr)
        {
            QSwitchWidget *widget = new QSwitchWidget(aSwitch->getName(),aSwitch->isReadOnly());

            m_switchBoard->layout()->addWidget(widget);

        }
    }

    ui->sensorScroller->setWidget(m_switchBoard);




    //    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//    dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));
//dummy->layout()->addWidget(new QSwitchWidget("Switch A",this));

}

void MainWindow::onDisconnected()
{
    ui->actionConnect->setEnabled(true);
    ui->actionDisconnect->setEnabled(false);
    ui->statusBar->setStyleSheet("background-color: red");
    m_statusBarLabel->setText("Not connected..");
    m_statusBarLabel->setStyleSheet("color: white");
}



QFruitHapSensor* MainWindow::getSensorByName(const QString name) const
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

void MainWindow::updateImage(QFruitHapSensor* cameraSensor)
{
    if (cameraSensor != nullptr)
    {
        cameraSensor->getValue();
    }
}



void MainWindow::on_cmbSensorList_currentIndexChanged(int index)
{
    QString selectedItem = ui->cmbSensorList->itemText(index);
    m_selectedSensor = getSensorByName(selectedItem);
    if (m_selectedSensor != nullptr)
    {
        ui->btnGetValue->setEnabled(m_selectedSensor->isPollable());

        QString sensorType(m_selectedSensor->metaObject()->className());
        if (sensorType == "QSwitch")
        {
            ui->tabWidget->setEnabled(false);
            ui->tabSwitch->setEnabled(true);
            ui->btnOn->setEnabled(!m_selectedSensor->isReadOnly());
            ui->btnOff->setEnabled(!m_selectedSensor->isReadOnly());
        }

        if (sensorType == "QCamera")
        {
           ui->tabSwitch->setEnabled(false);
            ui->tabCamera->setEnabled(true);
        }
    }

}



void MainWindow::onSwitchStateReceived(const QString name, SwitchState state)
{

    if (m_selectedSensor != nullptr && m_selectedSensor->getName() == name)
    {
      ui->lbState->setText(convertEnumToString(state));
    }

}

void MainWindow::onErrorReceived(const QString name, const QString errorMessage)
{
    qCritical() << "Error received from " << name << ". Message: " << errorMessage;
}


void MainWindow::onImageDataReceived(const QString name, const QByteArray imageData)
{
    if (m_selectedSensor != nullptr && m_selectedSensor->getName() == name)
    {
        QImage image = QImage::fromData(imageData);
        if (m_drawing != nullptr)
        {
            delete m_drawing;
            m_drawing = nullptr;
        }

        m_drawing = new QGraphicsScene(this);
        m_drawing->addPixmap(QPixmap::fromImage(image));
        m_drawing->setSceneRect(image.rect());

        ui->cameraImage->setScene(m_drawing);
    }
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

void MainWindow::on_dialRefreshrate_valueChanged(int value)
{
    m_timer->start(value);
}

void MainWindow::on_btnGetValue_clicked()
{
    if (m_selectedSensor != nullptr)
    {
        m_selectedSensor->getValue();

    }
}


void MainWindow::on_btnOn_clicked()
{
    QSwitch* aSwitch = dynamic_cast<QSwitch*>(m_selectedSensor);

    if (aSwitch != nullptr)
    {
        aSwitch->turnOn();
    }
}

void MainWindow::on_btnOff_clicked()
{
    QSwitch* aSwitch = dynamic_cast<QSwitch*>(m_selectedSensor);

    if (aSwitch != nullptr)
    {
        aSwitch->turnOff();
    }
}

void MainWindow::loadSensors()
{    
    m_configControl.getSensorList();


}
