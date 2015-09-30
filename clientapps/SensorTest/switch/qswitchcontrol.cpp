#include "qswitchcontrol.h"
#include <QJsonObject>
#include <QDateTime>

QSwitchControl::QSwitchControl(QFruitHapClient &client, QObject *parent):
   QObject(parent), m_isBusy(false), m_isConnected(false), m_client(client)
{
    connect(&m_client,&QFruitHapClient::responseReceived,this,&QSwitchControl::onClientResponseReceived);

}

void QSwitchControl::turnOn(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        qCritical() << "QSwitchControl::turnOn| Not connected or busy";
        return;
    }

    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOn";
    commandObject["Parameters"] = QJsonValue::Null;

    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);
    m_client.sendMessage(message);

}

void QSwitchControl::turnOff(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        qCritical() << "QSwitchControl::turnOff| Not connected or busy";
        return;
    }

    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOff";
    commandObject["Parameters"] = QJsonValue::Null;
    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);
    m_client.sendMessage(message);
}

void QSwitchControl::connectToServer(const QString &uri)
{
    if (!m_isConnected)
    {
        m_client.disconnectFromServer();
    }

    m_isConnected = m_client.connectToServer(uri);
}

void QSwitchControl::getState(const QString &name)
{
    if (!m_isConnected || m_isBusy)
    {
        qCritical() << "QSwitchControl::getState| Not connected or busy";
        return;
    }

    m_isBusy = true;
    m_currentName = name;

    QJsonObject obj;

    obj["SensorName"] = name;
    obj["EventType"] = "GetValue";

    QJsonDocument message(obj);
    m_client.sendMessage(message);
}

void QSwitchControl::getNames(std::vector<QString> &list)
{
    list.clear();
    list.push_back("SingleSocket");
    list.push_back("MultipleSocket");
}


void QSwitchControl::onClientResponseReceived(const QJsonDocument response)
{
    qDebug() << "QSwitchControl::onClientResponseReceived| Response received";
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QSwitchControl::onClientResponseReceived| Response is empty";
        m_isBusy = false;
        return;
    }

    QJsonObject responseObject = response.object();
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QSwitchControl::onClientResponseReceived| Response object is empty";
        m_isBusy = false;
        return;
    }

    if (responseObject["EventType"] == "ErrorMessage")
    {
        QString message(responseObject["Data"].toString());
        qCritical() << "Error response received " << message;
        m_isBusy = false;
        return;
    }

    if ( (responseObject["EventType"] == "GetValue" || responseObject["EventType"] == "SensorEvent"))
    {        
        int state = responseObject["Data"].toInt();
        m_state = static_cast<SwitchState>(state);

        if (responseObject["EventType"] == "GetValue" && responseObject["SensorName"] == m_currentName)
        {
            emit switchStateReceived(m_currentName,m_state);
            m_currentName.clear();
        }

        if (responseObject["EventType"] == "SensorEvent")
        {
            emit switchStateReceived(responseObject["SensorName"].toString(),m_state);
        }
    }

    m_isBusy = false;


}

