#include "qcameracontrol.h"
#include <QJsonObject>
#include <QJsonArray>

QCameraControl::QCameraControl(QString category, QFruitHapClient &client, QObject *parent) :
    QObject(parent), m_isBusy(false), m_category(category), m_client(client)
{
    m_requestTimer = new QTimer(this);
    m_requestTimer->setSingleShot(true);
    connect(m_requestTimer,&QTimer::timeout, this, &QCameraControl::requestTimeout);
    connect(&m_client,&QFruitHapClient::responseReceived,this,&QCameraControl::onClientResponseReceived);
}

void QCameraControl::getImage(const QString &name)
{
    if (m_isBusy)
    {
        qCritical() << "QCameraControl::getImage| Busy";
        return;
    }

    m_requestTimer->start(2000);
    m_isBusy = true;
    m_currentName = name;

    QJsonObject obj;

    obj["SensorName"] = name;
    obj["EventType"] = "GetValue";

    QJsonDocument message(obj);
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");
    m_client.sendMessage(message,routingKey,messageType);
}


void QCameraControl::handleSensorMessage(QJsonObject responseObject)
{
    if (responseObject["EventType"] == "ErrorMessage")
    {
        QString message(responseObject["Data"].toString());
        qCritical() << "Error response received " << message;
        return;
    }

    if (responseObject["EventType"] == "GetValue" && responseObject["SensorName"] == m_currentName)
    {
        QByteArray base64Data = responseObject["Data"].toObject()["$value"].toString().toLatin1();
        QByteArray data = QByteArray::fromBase64(base64Data);
        emit imageDataReceived(m_currentName,data);
        m_currentName.clear();
    }
}

void QCameraControl::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{
    qDebug() << "QCameraControl::onClientResponseReceived| Response received: " << messageType;
    m_requestTimer->stop();
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response is empty";
        m_isBusy = false;
        return;
    }

    QJsonObject responseObject = response.object();
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response object is empty";
        m_isBusy = false;
        return;
    }

    auto topics = m_client.getPubSubTopics();
    if (messageType.contains("SensorMessage") || topics.contains(messageType))
    {
        handleSensorMessage(responseObject);
    }


    m_isBusy = false;
}

void QCameraControl::requestTimeout()
{
    qCritical() << "Request timeout. Check your connection";
    m_isBusy = false;

}


