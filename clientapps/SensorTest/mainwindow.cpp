#include "mainwindow.h"
#include "ui_mainwindow.h"


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow),
    m_client(QString("FruitHAP_RpcExchange"), QString("FruitHAP_RpcQueue.FruitHap.Core.Action.SensorMessage"),QString("FruitHAP_PubSubExchange"),parent),
    m_switchControl(m_client,parent)
{

    ui->setupUi(this);

    QString uri("");
    QStringList bindingKeys;
    bindingKeys.push_back("alerts");
    m_client.setBindingKeys(bindingKeys);

    m_switchControl.connectToServer(uri);
    connect(&m_switchControl,&QSwitchControl::switchStateReceived,this,&MainWindow::onSwitchStateReceived);

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


void MainWindow::on_cmbSwitchList_currentIndexChanged(int index)
{
    QString selectedItem = ui->cmbSwitchList->itemText(index);
    m_switchControl.getState(selectedItem);
}

void MainWindow::on_btnGetState_clicked()
{

}

void MainWindow::onSwitchStateReceived(const QString name, SwitchState state)
{
  QString selectedItem = ui->cmbSwitchList->currentText();
  if (selectedItem == name)
  {
      ui->lbState->setText(convertEnumToString(state));
  }
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

void MainWindow::on_btnGetSwitchList_clicked()
{
    QStringList list;
    std::vector<QString> items;
    m_switchControl.getNames(items);
    for(uint i = 0; i < items.size();i++)
    {
        list.push_back(items[i]);
    }
    ui->cmbSwitchList->clear();
    ui->cmbSwitchList->addItems(list);

}
