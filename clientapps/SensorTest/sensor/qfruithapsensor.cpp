#include "qfruithapsensor.h"
#include <QJsonObject>


QFruitHapSensor::QFruitHapSensor(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QObject(parent), m_client(client), m_name(name), m_isPollable(isPollable), m_isReadOnly(isReadOnly)
{
    connect(m_client, &QFruitHapClient::responseReceived, this, &QFruitHapSensor::onClientResponseReceived);
}

void QFruitHapSensor::sendRequest(const QJsonObject request)
{
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client->sendMessage(request,routingKey,messageType);
}

void QFruitHapSensor::handleGetValueEvent(const QJsonObject)
{

}


void QFruitHapSensor::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{

    qDebug() << this->metaObject()->className() << "::onClientResponseReceived| Response received: " << messageType;
     if (response.isNull() || response.isEmpty())
     {
         qCritical() << this->metaObject()->className() << "::onClientResponseReceived| Response is empty";

         return;
     }

     QJsonObject responseObject = response.object();
     if (response.isNull() || response.isEmpty())
     {
         qDebug() << this->metaObject()->className() << "::onClientResponseReceived| Response object is empty";

         return;
     }

     auto topics = m_client->getPubSubTopics();

     if ( (topics.contains(messageType) || messageType.contains("SensorMessage")) && responseObject["SensorName"] == m_name)
     {
         qDebug() << this->metaObject()->className() << "This response is for me: " << responseObject["SensorName"].toString();
         if (responseObject["EventType"] == "SensorEvent")
         {
            handleSensorEvent(responseObject);
         }

         if (responseObject["EventType"] == "GetValue")
         {
             handleGetValueEvent(responseObject);
         }

         if (responseObject["EventType"] == "ErrorMessage")
         {
             QString message = responseObject["Data"].toObject()["$value"].toString();
             QString name = responseObject["SensorName"].toString();
             emit errorEventReceived(message,name);
         }


     }



}



QString QFruitHapSensor::getName() const
{
    return m_name;
}

bool QFruitHapSensor::isPollable() const
{
    return m_isPollable;
}

bool QFruitHapSensor::isReadOnly() const
{
    return m_isReadOnly;
}

void QFruitHapSensor::getValue()
{
    if (!isPollable())
    {
        qWarning() << "This sensor is not pollable";
        return;
    }
    QJsonObject obj;
    obj["SensorName"] = m_name;
    obj["EventType"] = "GetValue";
    sendRequest(obj);
}

