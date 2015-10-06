#include "qeventedswitch.h"
#include <QJsonObject>



QEventedSwitch::QEventedSwitch(QFruitHapClient *client, QString name, QObject *parent):
    QEventedSensor(client,name,parent)
{

}

void QEventedSwitch::turnOn()
{
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

