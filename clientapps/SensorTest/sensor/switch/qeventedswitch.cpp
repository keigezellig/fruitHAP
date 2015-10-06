#include "qeventedswitch.h"
#include <QJsonObject>



QEventedSwitch::QEventedSwitch(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QEventedSensor(client,name,isPollable,isReadOnly,parent)
{

}

void QEventedSwitch::turnOn()
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

void QEventedSwitch::turnOff()
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



void QEventedSwitch::sendSignal(const QJsonObject &responseObject)
{
    int state = responseObject["Data"].toInt();
    SwitchState theState = static_cast<SwitchState>(state);
    emit switchStateReceived(m_name,theState);
}

void QEventedSwitch::handleSensorEvent(const QJsonObject responseObject)
{
    sendSignal(responseObject);
}

void QEventedSwitch::handleGetValueEvent(const QJsonObject responseObject)
{
    sendSignal(responseObject);
}

