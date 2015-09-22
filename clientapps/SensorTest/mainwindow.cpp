#include "mainwindow.h"
#include "ui_mainwindow.h"


MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);   
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
    SwitchState state = m_switchControl.getState(selectedItem.toStdString());
    ui->lbState->setText(convertEnumToString(state));
}

void MainWindow::on_switchControl_switchStateChanged(const std::string switchName, const SwitchState newState)
{
    ui->lbState->setText(convertEnumToString(newState));
}


void MainWindow::on_btnOn_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    m_switchControl.turnOn(selectedItem.toStdString());
}

void MainWindow::on_btnOff_clicked()
{
    QString selectedItem = ui->cmbSwitchList->itemText(ui->cmbSwitchList->currentIndex());
    m_switchControl.turnOff(selectedItem.toStdString());
}

void MainWindow::on_btnGetSwitchList_clicked()
{
    QStringList list;
    std::vector<std::string> items = m_switchControl.getSwitchNames();
    for(uint i = 0; i < items.size();i++)
    {
        list.push_back(QString::fromStdString(items[i]));
    }
    ui->cmbSwitchList->clear();
    ui->cmbSwitchList->addItems(list);

}
