#include "qsensor.h"
#include <QJsonObject>


QSensor::QSensor(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QObject(parent), m_client(client), m_name(name), m_isPollable(isPollable), m_isReadOnly(isReadOnly)
{
    connect(m_client, &QFruitHapClient::responseReceived, this, &QSensor::onClientResponseReceived);
}

void QSensor::sendRequest(const QJsonObject request)
{
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client->sendMessage(request,routingKey,messageType);
}

void QSensor::handleGetValueEvent(const QJsonObject)
{

}


void QSensor::onClientResponseReceived(const QJsonDocument response, const QString messageType)
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



QString QSensor::getName() const
{
    return m_name;
}

bool QSensor::isPollable() const
{
    return m_isPollable;
}

bool QSensor::isReadOnly() const
{
    return m_isReadOnly;
}

void QSensor::getValue()
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

