#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "sensor/camera/qeventedcamera.h"
#include "sensor/switch/qeventedswitch.h"
#include <QInputDialog>
#include <QPixmap>


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_drawing(nullptr),
    m_client(new QFruitHapClient(QString("FruitHAP_RpcExchange"), QString("FruitHAP_PubSubExchange"),this)),
    m_configControl(m_client,this),
    m_eventedSensors()


{

    ui->setupUi(this);


    connect(&m_configControl,&QConfigurationControl::sensorListReceived,this,&MainWindow::onSensorListReceived);
    connect(m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);
    connect(m_client,&QFruitHapClient::rpcQueueReady,this,&MainWindow::onRpcQueueReady);
    m_timer = new QTimer(this);
    connect(m_timer,&QTimer::timeout, this, &MainWindow::updateImage);
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
    if (isConnected)
    {
        onConnected();
    }
    else
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
}

void MainWindow::onSensorListReceived(const QList<SensorData> list)
{

    ui->cmbSwitchList->clear();
    ui->cmbCameraList->clear();

    foreach (SensorData item, list)
    {
        if (item.getCategory() == "Switch")
        {            
            QEventedSwitch *eventedSwitch = new QEventedSwitch(m_client,item.getName(),true,item.IsReadOnly(),parent());
            connect(eventedSwitch, &QEventedSwitch::switchStateReceived, this, &MainWindow::onSwitchStateReceived);
            connect(eventedSwitch, &QEventedSwitch::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedSwitch);
            ui->cmbSwitchList->addItem(item.getName());
        }


        if( (item.getType() == "ButtonWithCameraSensor") || (item.getType() == "SwitchWithCameraSensor"))
        {

            QEventedCamera *eventedCamera = new QEventedCamera(m_client,item.getName(),false,item.IsReadOnly(),parent());
            connect(eventedCamera, &QEventedCamera::imageReceived, this, &MainWindow::onImageDataReceived);
            connect(eventedCamera, &QEventedCamera::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedCamera);
            ui->cmbCameraList->addItem(item.getName());
        }

        if( (item.getType() == "Camera"))
        {

            QEventedCamera *eventedCamera = new QEventedCamera(m_client,item.getName(),true,item.IsReadOnly(),parent());
            connect(eventedCamera, &QEventedCamera::imageReceived, this, &MainWindow::onImageDataReceived);
            connect(eventedCamera, &QEventedCamera::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_eventedSensors.append(eventedCamera);
            ui->cmbCameraList->addItem(item.getName());
        }
    }



}

void MainWindow::onDisconnected()
{
    ui->actionConnect->setEnabled(true);
    ui->actionDisconnect->setEnabled(false);
}

void MainWindow::onRpcQueueReady()
{
    loadSensors();
}


QEventedSensor* MainWindow::getSensorByName(const QString name) const
{
    foreach (QEventedSensor* item, m_eventedSensors)
    {
        if (item->getName() == name)
        {
            return item;
        }
    }

    return nullptr;
}

void MainWindow::updateImage()
{
    QString selectedItem = ui->cmbCameraList->itemText(ui->cmbCameraList->currentIndex());
    QEventedSensor* selectedSensor = getSensorByName(selectedItem);

    if (selectedSensor != nullptr && selectedSensor->isPollable())
    {
        selectedSensor->getValue();
    }
}



void MainWindow::on_cmbSwitchList_currentIndexChanged(int index)
{
    QString selectedItem = ui->cmbSwitchList->itemText(index);
    QEventedSensor* selectedSensor = getSensorByName(selectedItem);

    if (selectedSensor != nullptr && selectedSensor->isPollable())
    {
        selectedSensor->getValue();
        ui->btnOn->setEnabled(!selectedSensor->isReadOnly());
        ui->btnOff->setEnabled(!selectedSensor->isReadOnly());
    }


}

void MainWindow::on_cmbCameraList_currentIndexChanged(int index)
{
    updateImage();
}


void MainWindow::onSwitchStateReceived(const QString name, SwitchState state)
{
  QString selectedItem = ui->cmbSwitchList->currentText();
  if (selectedItem == name)
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
    QString selectedItem = ui->cmbCameraList->currentText();
    if (selectedItem == name)
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
    QString uri = QInputDialog::getText(this,"Connect to MQ server",
                                            "Enter AMQ connection string (default is amqp://guest:guest@localhost)", QLineEdit::Normal,
                                            "", &ok);
    if (ok)
    {
            QStringList bindingKeys;
            bindingKeys.push_back("alerts");

            connectToMQ(bindingKeys, uri);

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


void MainWindow::on_btnOn_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    QEventedSensor* selectedSensor = getSensorByName(selectedItem);
    QEventedSwitch* aSwitch = dynamic_cast<QEventedSwitch*>(selectedSensor);

    if (aSwitch != nullptr)
    {
        aSwitch->turnOn();
    }
}

void MainWindow::on_btnOff_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    QEventedSensor* selectedSensor = getSensorByName(selectedItem);
    QEventedSwitch* aSwitch = dynamic_cast<QEventedSwitch*>(selectedSensor);

    if (aSwitch != nullptr)
    {
        aSwitch->turnOff();
    }
}

void MainWindow::loadSensors()
{    
    m_configControl.getSensorNames();


}
