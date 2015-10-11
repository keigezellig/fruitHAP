#include "qswitchwidget.h"
#include "ui_qswitchwidget.h"

QSwitchWidget::QSwitchWidget(const QString name, const bool isReadOnly, QWidget *parent) :
    QWidget(parent),
    ui(new Ui::QSwitchWidget)
{
    ui->setupUi(this);
    setName(name);
}

QSwitchWidget::~QSwitchWidget()
{
    delete ui;
}

void QSwitchWidget::setName(const QString &name)
{
    ui->lblName->setText(name);
}

void QSwitchWidget::setIsReadOnly(bool value)
{
    if (value)
    {
        ui->btnOn->setEnabled(false);
        ui->btnOff->setEnabled(false);
    }
    else
    {
        ui->btnOn->setEnabled(true);
        ui->btnOff->setEnabled(true);
    }
}

void QSwitchWidget::onStateChanged(const State newState)
{
   QString text("Undefined");
   QString styleSheet("color: black");

   convertEnumToString(newState,text,styleSheet);
   ui->lblState->setText(text);
   ui->lblState->setStyleSheet(styleSheet);
}

void QSwitchWidget::on_btnOff_clicked()
{
    emit turnOn();
}

void QSwitchWidget::on_btnOn_clicked()
{
    emit turnOff();
}

void QSwitchWidget::convertEnumToString(const State& state, QString& string, QString& styleSheet )
{
    switch (state)
    {
    case State::On:
        string = "On";
        styleSheet = "color: green";
    case State::Off:
       string = "Off";
       styleSheet = "color: red";
    case State::Undefined:
       string = "Undefined";
       styleSheet = "color: black";
    }

}
