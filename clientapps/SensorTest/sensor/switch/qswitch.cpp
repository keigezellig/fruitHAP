#include "qswitch.h"
#include <QJsonObject>
#include <QDateTime>


QSwitch::QSwitch(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QFruitHapSensor(client,name,isPollable,isReadOnly,parent)
{

}

void QSwitch::turnOn()
{
    if (isReadOnly())
    {
        qWarning() << "This is a readonly sensor";
        return;
    }
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOn";
    commandObject["Parameters"] = QJsonValue::Null;

    obj["SensorName"] = m_name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    sendRequest(obj);
}

void QSwitch::turnOff()
{
    if (isReadOnly())
    {
        qWarning() << "This is a readonly sensor";
        return;
    }

    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOff";
    commandObject["Parameters"] = QJsonValue::Null;

    obj["SensorName"] = m_name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;
    sendRequest(obj);
}



void QSwitch::sendSignal(const QJsonObject &responseObject)
{
     int state = responseObject["Data"].toInt();
    QDateTime timestamp = QDateTime::fromString(responseObject["TimeStamp"].toString(),Qt::ISODate);
    SwitchState theState = static_cast<SwitchState>(state);
    emit switchStateReceived(m_name, theState, timestamp);
}

void QSwitch::handleSensorEvent(const QJsonObject responseObject)
{
    sendSignal(responseObject);
}

void QSwitch::handleGetValueEvent(const QJsonObject responseObject)
{
    sendSignal(responseObject);
}

