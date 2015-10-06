#include "qeventedcamera.h"
#include <QJsonObject>



QEventedCamera::QEventedCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent):
    QEventedSensor(client,name,isPollable,isReadOnly,parent)
{

}

void QEventedCamera::handleSensorEvent(const QJsonObject responseObject)
{
    QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
    QByteArray data = QByteArray::fromBase64(base64Data);
    emit imageReceived(m_name,data);

}

void QEventedCamera::handleGetValueEvent(const QJsonObject responseObject)
{
    QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
    QByteArray data = QByteArray::fromBase64(base64Data);
    emit imageReceived(m_name,data);
}
