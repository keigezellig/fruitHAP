#include "qswitch.h"

QSwitch::QSwitch(const QString name, QObject *parent) :QObject(parent), m_name(name), m_state(SwitchState::Undefined)
{

}

void QSwitch::turnOn()
{

}

void QSwitch::turnOff()
{

}

QString QSwitch::getName() const
{
     return m_name;
}

SwitchState QSwitch::getState() const
{
    return m_state;
}

void QSwitch::onMessageReceived(const QJsonDocument message)
{
    if (!message.isEmpty())
    {
        QJsonObject& obj = message.object();

    }
}

