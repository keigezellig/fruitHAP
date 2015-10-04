#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QInputDialog>
#include <QPixmap>


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_drawing(nullptr),
    m_client(QString("FruitHAP_RpcExchange"), QString("FruitHAP_PubSubExchange"),parent),
    m_switchControl(QString("Switch"),m_client,parent),
    m_cameraControl(QString("Camera"),m_client,parent),
    m_configControl(m_client,parent)


{

    ui->setupUi(this);

    connect(&m_switchControl,&QSwitchControl::switchStateReceived,this,&MainWindow::onSwitchStateReceived);
    connect(&m_cameraControl,&QCameraControl::imageDataReceived,this,&MainWindow::onImageDataReceived);
    connect(&m_configControl,&QConfigurationControl::sensorListReceived,this,&MainWindow::onSensorListReceived);
    connect(&m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(&m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);
    connect(&m_client,&QFruitHapClient::rpcQueueReady,this,&MainWindow::onRpcQueueReady);    
    m_timer = new QTimer(this);
    connect(m_timer,&QTimer::timeout, this, &MainWindow::updateImage);


}

MainWindow::~MainWindow()
{
    delete m_timer;
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
    m_client.setPubSubTopics(bindingKeys);
    m_client.connectToServer(uri);

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
            ui->cmbSwitchList->addItem(item.getName());
        }

        if (item.getCategory() == "Camera")
        {
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

void MainWindow::updateImage()
{
    QString selectedItem = ui->cmbCameraList->itemText(ui->cmbCameraList->currentIndex());
    m_cameraControl.getImage(selectedItem);
}



void MainWindow::on_cmbSwitchList_currentIndexChanged(int index)
{
    QString selectedItem = ui->cmbSwitchList->itemText(index);
    m_switchControl.getState(selectedItem);
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
    m_client.disconnectFromServer();
}

void MainWindow::on_dialRefreshrate_valueChanged(int value)
{
    m_timer->start(value);
}


void MainWindow::on_btnOn_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    m_switchControl.turnOn(selectedItem);
}

void MainWindow::on_btnOff_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    m_switchControl.turnOff(selectedItem);
}

void MainWindow::loadSensors()
{    
    m_configControl.getSensorNames();


}
