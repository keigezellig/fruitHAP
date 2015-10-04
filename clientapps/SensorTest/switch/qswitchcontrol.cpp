#include "qswitchcontrol.h"
#include <QJsonObject>
#include <QDateTime>
#include <QJsonArray>

QSwitchControl::QSwitchControl(QString category, QFruitHapClient &client, QObject *parent):
   QObject(parent), m_isBusy(false), m_category(category), m_client(client)
{    
    m_requestTimer = new QTimer(this);
    m_requestTimer->setSingleShot(true);
    connect(m_requestTimer,&QTimer::timeout, this, &QSwitchControl::requestTimeout);
    connect(&m_client,&QFruitHapClient::responseReceived,this,&QSwitchControl::onClientResponseReceived);


}

void QSwitchControl::turnOn(const QString &name)
{
    if (m_isBusy)
    {
        qCritical() << "QSwitchControl::turnOn| Busy";
        return;
    }
    m_requestTimer->start(2000);
    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOn";
    commandObject["Parameters"] = QJsonValue::Null;

    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);

    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client.sendMessage(message,routingKey,messageType);

}

void QSwitchControl::turnOff(const QString &name)
{
    if (m_isBusy)
    {
        qCritical() << "QSwitchControl::turnOff| busy";
        return;
    }
    m_requestTimer->start(2000);
    m_isBusy = true;
    QJsonObject obj;
    QJsonObject commandObject;

    commandObject["OperationName"] = "TurnOff";
    commandObject["Parameters"] = QJsonValue::Null;
    obj["SensorName"] = name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;

    QJsonDocument message(obj);
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client.sendMessage(message,routingKey,messageType);
}

void QSwitchControl::getState(const QString &name)
{
    if (m_isBusy)
    {
        qCritical() << "QSwitchControl::getState| Busy";
        return;
    }

    m_requestTimer->start(2000);
    m_isBusy = true;
    m_currentName = name;

    QJsonObject obj;

    obj["SensorName"] = name;
    obj["EventType"] = "GetValue";

    QJsonDocument message(obj);
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client.sendMessage(message,routingKey,messageType);
}





void QSwitchControl::handleSensorMessage(QJsonObject responseObject)
{
    if (responseObject["EventType"] == "ErrorMessage")
    {
        QString message(responseObject["Data"].toString());
        qCritical() << "Error response received " << message;
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


}

void QSwitchControl::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{    

   qDebug() << "QSwitchControl::onClientResponseReceived| Response received: " << messageType;
    m_requestTimer->stop();
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

    auto topics = m_client.getPubSubTopics();

    if (messageType.contains("SensorMessage") || topics.contains(messageType))
    {
        handleSensorMessage(responseObject);
    }

    m_isBusy = false;


}

void QSwitchControl::requestTimeout()
{
    qCritical() << "Request timeout. Check your connection";
    m_isBusy = false;
}

