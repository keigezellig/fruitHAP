#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QInputDialog>



MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_client(QString("FruitHAP_RpcExchange"), QString("FruitHAP_RpcQueue.FruitHap.Core.Action.SensorMessage"),QString("FruitHAP_PubSubExchange"),parent),
    m_switchControl(m_client,parent)
{

    ui->setupUi(this);

    connect(&m_switchControl,&QSwitchControl::switchStateReceived,this,&MainWindow::onSwitchStateReceived);
    connect(&m_switchControl,&QSwitchControl::switchListReceived,this,&MainWindow::onSwitchListReceived);
    connect(&m_client,&QFruitHapClient::connected,this,&MainWindow::onConnected);
    connect(&m_client,&QFruitHapClient::disconnected,this,&MainWindow::onDisconnected);
    connect(&m_client,&QFruitHapClient::rpcQueueReady,this,&MainWindow::onRpcQueueReady);

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
    m_client.setPubSubTopics(bindingKeys);
    m_client.connectToServer(uri);

}

void MainWindow::onConnected()
{
    ui->actionConnect->setEnabled(false);
    ui->actionDisconnect->setEnabled(true);
}

void MainWindow::onDisconnected()
{
    ui->actionConnect->setEnabled(true);
    ui->actionDisconnect->setEnabled(false);
}

void MainWindow::onRpcQueueReady()
{
    loadSwitches();
}



void MainWindow::on_cmbSwitchList_currentIndexChanged(int index)
{
    QString selectedItem = ui->cmbSwitchList->itemText(index);
    m_switchControl.getState(selectedItem);
}


void MainWindow::onSwitchStateReceived(const QString name, SwitchState state)
{
  QString selectedItem = ui->cmbSwitchList->currentText();
  if (selectedItem == name)
  {
      ui->lbState->setText(convertEnumToString(state));
  }
}

void MainWindow::onSwitchListReceived(const QStringList list)
{
    ui->cmbSwitchList->clear();
    ui->cmbSwitchList->addItems(list);
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

void MainWindow::loadSwitches()
{
    QStringList list;
    std::vector<QString> items;
    m_switchControl.getNames();
//    for(uint i = 0; i < items.size();i++)
//    {
//        list.push_back(items[i]);
//    }
//    ui->cmbSwitchList->clear();
//    ui->cmbSwitchList->addItems(list);

}
