#include "qcamera.h"
#include <QJsonObject>



QCamera::QCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QFruitHapSensor(client,name,isPollable,isReadOnly,parent)
{

}

void QCamera::handleSensorEvent(const QJsonObject responseObject)
{
    QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
    QDateTime timestamp = QDateTime::fromString(responseObject["TimeStamp"].toString(),Qt::ISODate);
    QByteArray data = QByteArray::fromBase64(base64Data);
    emit imageReceived(m_name,data, timestamp);

}

void QCamera::handleGetValueEvent(const QJsonObject responseObject)
{
    QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
    QDateTime timestamp = QDateTime::fromString(responseObject["TimeStamp"].toString(),Qt::ISODate);
    QByteArray data = QByteArray::fromBase64(base64Data);
    emit imageReceived(m_name,data, timestamp);
}
