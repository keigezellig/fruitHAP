#include "qeventedcamera.h"



QEventedCamera::QEventedCamera(QFruitHapClient *client, QString name, QObject *parent):
    QEventedSensor(client,name,parent)
{

}

QEventedCamera::handleSensorEvent(QJsonObject responseObject)
{
    QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
    QByteArray data = QByteArray::fromBase64(base64Data);
    emit imageDataReceived(m_currentName,data);

}
