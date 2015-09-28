#include "qswitchcontrol.h"
#include <QJsonObject>
#include <QDateTime>

QSwitchControl::QSwitchControl(std::shared_ptr<QFruitHapClient> client, QObject *parent):
   QObject(parent), m_isBusy(false), m_isConnected(false), m_client(std::move(client))
{

}

void QSwitchControl::turnOn(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        return;
    }

    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOn";
    commandObject["Parameters"] = QJsonValue::Null;
    obj["TimeStamp"] = QDateTime::currentMSecsSinceEpoch();
    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);
    m_client->sendMessage(message);

}

void QSwitchControl::turnOff(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        return;
    }

    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOn";
    commandObject["Parameters"] = QJsonValue::Null;
    obj["TimeStamp"] = QDateTime::currentMSecsSinceEpoch();
    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);
    m_client->sendMessage(message);
}

void QSwitchControl::connect(const QString &uri)
{
    if (!m_isConnected)
    {
        m_client->disconnectFromServer();
    }

    m_isConnected = m_client->connectToServer(uri);
}

SwitchState QSwitchControl::getState(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        return SwitchState::Undefined;
    }

    m_isBusy = true;
    QJsonObject obj;

    obj["TimeStamp"] = QDateTime::currentMSecsSinceEpoch();
    obj["SensorName"] = name;
    obj["EventType"] = "GetValue";

    QJsonDocument message(obj);
    m_client->sendMessage(message);
    while (m_isBusy)
    {

    }

    return m_state;
}

void QSwitchControl::getNames(std::vector<QString> &list)
{
    list.clear();
    list.push_back("Een");
    list.push_back("Twee");
}


void QSwitchControl::onClientResponseReceived(const QJsonDocument response)
{
    if (response.isNull() || response.isEmpty())
    {
        m_isBusy = false;
        return;
    }

    QJsonObject responseObject = response.object();
    if (response.isNull() || response.isEmpty())
    {
        m_isBusy = false;
        return;
    }

    if (responseObject["EventType"] == "ErrorMessage")
    {
        m_isBusy = false;
        return;
    }

    if (responseObject["EventType"] == "GetValue")
    {
        int state = responseObject["Data"].toInt();
        m_state = static_cast<SwitchState>(state);
    }

    m_isBusy = true;


}

