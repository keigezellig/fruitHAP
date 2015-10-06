#include "qeventedsensor.h"
#include <QJsonObject>


QEventedSensor::QEventedSensor(QFruitHapClient *client, QString name, QObject *parent):
    QObject(parent), m_client(client),m_name(name)
{
    connect(client, &QFruitHapClient::responseReceived, this, &QEventedSensor::onClientResponseReceived)
}

void QEventedSensor::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{
    qDebug() << "QSwitchControl::onClientResponseReceived| Response received: " << messageType;
     if (response.isNull() || response.isEmpty())
     {
         qCritical() << "QSwitchControl::onClientResponseReceived| Response is empty";
         return;
     }

     QJsonObject responseObject = response.object();
     if (response.isNull() || response.isEmpty())
     {
         qCritical() << "QSwitchControl::onClientResponseReceived| Response object is empty";
         return;
     }

     auto topics = m_client->getPubSubTopics();

     if (topics.contains(messageType) && messageType.contains("SensorMessage") && responseObject["SensorName"] == m_name && responseObject["EventType"] == "SensorEvent")
     {
         handleSensorEvent(responseObject);
     }

}

QString QEventedSensor::getName() const
{
    return m_name;
}

