#include "qeventedsensor.h"
#include <QJsonObject>

QEventedSensor::QEventedSensor(QFruitHapClient *client, QObject *parent):
    QObject(parent), m_client(client)
{

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

     if (messageType.contains("SensorMessage") || topics.contains(messageType))
     {
         handleSensorMessage(responseObject);
     }

}
