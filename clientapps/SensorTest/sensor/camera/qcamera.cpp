#include "qcamera.h"
#include <QJsonObject>



QCamera::QCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, FaceVerifier *faceVerifier, QObject *parent):
    QFruitHapSensor(client,name,isPollable,isReadOnly,parent), m_isFaceDetectionEnabled(false), m_faceVerifier(faceVerifier)
{

}

void QCamera::enableFaceDetection(bool isEnabled)
{
    m_isFaceDetectionEnabled = isEnabled;
}

bool QCamera::isFaceDetectionEnabled() const
{
    return m_isFaceDetectionEnabled;
}

void QCamera::sendImage(const QJsonObject responseObject)
{
    QJsonObject dataObject = responseObject["Data"].toObject();
    QByteArray base64Data = dataObject["Content"].toObject()["$value"].toString().toLatin1();
    QDateTime timestamp = QDateTime::fromString(responseObject["TimeStamp"].toString(),Qt::ISODate);
    QByteArray data = QByteArray::fromBase64(base64Data);
    QByteArray imageWithFace;



    int faces = 0;
    if (m_faceVerifier != nullptr && m_isFaceDetectionEnabled)
    {
        faces = m_faceVerifier->DetectFacesInImage(data,imageWithFace);
        if (faces > 0)
        {
            qDebug() << "Face detected on camera: " << m_name;            
            emit faceDetected(m_name, data, timestamp);
        }
        else
        {
            qDebug() << "No face detected on camera: " << m_name;
            emit imageReceived(m_name,data, timestamp);
        }
    }
    else
    {
        emit imageReceived(m_name,data, timestamp);
    }



}

void QCamera::handleSensorEvent(const QJsonObject responseObject)
{
    sendImage(responseObject);

}

void QCamera::handleGetValueEvent(const QJsonObject responseObject)
{
    sendImage(responseObject);
}
