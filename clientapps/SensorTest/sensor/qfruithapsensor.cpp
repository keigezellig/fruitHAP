#include "qfruithapsensor.h"
#include <QJsonObject>


QFruitHapSensor::QFruitHapSensor(QFruitHapClient *client, FruitHapApi *apiClient, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QObject(parent), m_apiClient(apiClient), m_client(client), m_name(name), m_isPollable(isPollable), m_isReadOnly(isReadOnly)
{
    connect(m_client, &QFruitHapClient::responseReceived, this, &QFruitHapSensor::onClientResponseReceived);
    connect(m_apiClient, &FruitHapApi::sensorResponseReceived, this, &QFruitHapSensor::onApiResponseReceived );
}

void QFruitHapSensor::sendRequest(const QJsonObject request)
{
    QString sensorName = request["SensorName"].toString();
    QString operation = request["Data"].toObject()["OperationName"].toString();

    m_apiClient->requestSensorOperation(sensorName, operation);
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

     if ( (topics.contains(messageType) && responseObject["SensorName"] == m_name))
     {
         qDebug() << this->metaObject()->className() << "This response is for me: " << responseObject["SensorName"].toString();
         handleSensorEvent(responseObject);

     }

}

void QFruitHapSensor::onApiResponseReceived(const QJsonDocument response)
{
    QJsonObject responseObject = response.object();
    QJsonObject dataObject = responseObject["Data"].toObject();
    QString sensorName = responseObject["SensorName"].toString();

    if (sensorName == m_name && dataObject["TypeName"] != "CommandResult") {
        handleGetValueEvent(responseObject);
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
    QJsonObject commandObject;

    commandObject["OperationName"] = "GetValue";
    commandObject["Parameters"] = QJsonValue::Null;

    obj["SensorName"] = m_name;
    obj["EventType"] = "Command";
    obj["Data"] = commandObject;
    sendRequest(obj);
}

