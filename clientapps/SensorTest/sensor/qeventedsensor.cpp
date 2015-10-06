#include "qeventedsensor.h"
#include <QJsonObject>


QEventedSensor::QEventedSensor(QFruitHapClient *client, QString name, QObject *parent):
    QObject(parent),m_isBusy(false), m_client(client), m_name(name)
{
    connect(m_client, &QFruitHapClient::responseReceived, this, &QEventedSensor::onClientResponseReceived);
    connect(this, &QEventedSensor::responseHandled, this, &QEventedSensor::onResponseHandled);
    m_requestTimer = new QTimer(this);
    m_requestTimer->setSingleShot(true);
    connect(m_requestTimer,&QTimer::timeout, this, &QEventedSensor::onRequestTimeout);

}

void QEventedSensor::sendRequest(const QJsonObject request)
{

    if (m_isBusy)
    {
       qCritical() << this->metaObject()->className() << "::SendRequest: Busy";
        return;
    }
    m_requestTimer->start(2000);
    m_isBusy = true;
    QJsonDocument message(request);

    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client->sendMessage(message,routingKey,messageType);


}

void QEventedSensor::handleGetValueEvent(const QJsonObject)
{

}


void QEventedSensor::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{

    qDebug() << this->metaObject()->className() << "::onClientResponseReceived| Response received: " << messageType;
     if (response.isNull() || response.isEmpty())
     {
         qCritical() << this->metaObject()->className() << "::onClientResponseReceived| Response is empty";
         emit responseHandled();
         return;
     }

     QJsonObject responseObject = response.object();
     if (response.isNull() || response.isEmpty())
     {
         qDebug() << this->metaObject()->className() << "::onClientResponseReceived| Response object is empty";
         emit responseHandled();
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

         emit responseHandled();
     }



}

void QEventedSensor::onResponseHandled()
{
    qDebug() << this->metaObject()->className() << "Response handled";
    m_isBusy = false;
    if (m_requestTimer->isActive())
    {
        m_requestTimer->stop();
    }

}


void QEventedSensor::onRequestTimeout()
{
    qCritical() << "Request timeout. Check your connection";
    m_isBusy = false;

}

QString QEventedSensor::getName() const
{
    return m_name;
}

void QEventedSensor::getValue()
{
    QJsonObject obj;


    obj["SensorName"] = m_name;
    obj["EventType"] = "GetValue";
    sendRequest(obj);
}

