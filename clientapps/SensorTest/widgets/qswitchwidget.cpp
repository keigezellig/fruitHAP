#include "qswitchwidget.h"
#include "ui_qswitchwidget.h"


QSwitchWidget::QSwitchWidget(const QString name, const bool isReadOnly, const bool isPollable, QWidget *parent) :
    QWidget(parent),  m_name(name),
    ui(new Ui::QSwitchWidget)
{
    ui->setupUi(this);
    setName(m_name);
    setIsReadOnly(isReadOnly);
    setPollable(isPollable);
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
    ui->btnOn->setEnabled(!value);
    ui->btnOff->setEnabled(!value);
}

void QSwitchWidget::setPollable(bool value)
{
    ui->btnRefresh->setEnabled(value);
}

void QSwitchWidget::onStateChanged(const QString name, const SwitchState newState, const QDateTime timestamp)
{
   if (name == m_name)
   {
       QString text("Undefined");
       QString styleSheet("color: black");

       convertEnumToString(newState,text,styleSheet);
       ui->lblState->setText(text);
       ui->lblState->setStyleSheet(styleSheet);
       ui->lblLastRefreshed->setText(timestamp.toString());
   }
}

void QSwitchWidget::on_btnOff_clicked()
{
    emit turnOff();
}

void QSwitchWidget::on_btnOn_clicked()
{
    emit turnOn();
}

void QSwitchWidget::on_btnRefresh_clicked()
{
    emit refresh();
}

void QSwitchWidget::convertEnumToString(const SwitchState &state, QString &string, QString &styleSheet )
{
    switch (state)
    {
    case SwitchState::On:
        string = "On";
        styleSheet = "color: green";
        break;
    case SwitchState::Off:
       string = "Off";
       styleSheet = "color: red";
        break;
    case SwitchState::Undefined:
       string = "Undefined";
       styleSheet = "color: black";
        break;
    }

}
