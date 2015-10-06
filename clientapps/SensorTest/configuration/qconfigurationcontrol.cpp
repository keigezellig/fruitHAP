#include "qconfigurationcontrol.h"


QConfigurationControl::QConfigurationControl(QFruitHapClient *client, QObject *parent):
    QObject(parent), m_client(client), m_isBusy(false)
{
    m_requestTimer = new QTimer(this);
    m_requestTimer->setSingleShot(true);
    connect(m_requestTimer,&QTimer::timeout, this, &QConfigurationControl::requestTimeout);
    connect(m_client,&QFruitHapClient::responseReceived,this,&QConfigurationControl::onClientResponseReceived);

}

QConfigurationControl::QConfigurationControl(const QConfigurationControl &copy):
    QObject(copy.parent()),m_client(copy.m_client), m_isBusy(copy.m_isBusy)
{
    m_requestTimer = new QTimer(this);
    m_requestTimer->setSingleShot(true);
    connect(m_requestTimer,&QTimer::timeout, this, &QConfigurationControl::requestTimeout);
    connect(m_client,&QFruitHapClient::responseReceived,this,&QConfigurationControl::onClientResponseReceived);
}

void QConfigurationControl::getSensorNames()
{

    qDebug() << "Get sensor list";

    if (m_isBusy)
    {
        qCritical() << "Get sensor list| Busy";
        return;
    }

    m_requestTimer->start(2000);
    m_isBusy = true;

    QJsonObject obj;
    QJsonObject paramObj;

    paramObj["Category"] = m_category;

    obj["OperationName"] = "GetAllSensors";
    obj["Parameters"] = paramObj;
    obj["MessageType"] = 0;

    QJsonDocument message(obj);
    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.ConfigurationMessage");
    QString messageType("FruitHAP.Core.Action.ConfigurationMessage:FruitHAP.Core");
    m_client->sendMessage(message,routingKey,messageType);
}

void QConfigurationControl::handleConfigurationMessage(QJsonObject responseObject)
{

    if (responseObject["MessageType"] == 2)
    {
        QString message(responseObject["Data"].toString());
        qCritical() << "Error response received " << message;
        return;
    }

    if (responseObject["OperationName"] == "GetAllSensors")
    {
        QList<SensorData> sensorDataList;
        QJsonObject dataObject = responseObject["Data"].toObject();
        QJsonArray sensorList = dataObject["$values"].toArray();
        foreach(auto sensor, sensorList)
        {
            QJsonObject sensorObject = sensor.toObject();
            QJsonObject parameters = sensorObject["Parameters"].toObject();
            SensorData data(parameters["Name"].toString(),parameters["Category"].toString(), sensorObject["Type"].toString(), parameters["IsReadOnly"].toBool() );
            sensorDataList.append(data);

        }

        emit sensorListReceived(sensorDataList);


    }


}

void QConfigurationControl::onClientResponseReceived(const QJsonDocument response, const QString messageType)
{
    qDebug() << "QConfigurationControl::onClientResponseReceived| Response received " << messageType;
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

    if (messageType.contains("ConfigurationMessage"))
    {
        handleConfigurationMessage(responseObject);

    }
}

void QConfigurationControl::requestTimeout()
{
    qCritical() << "Request timeout. Check your connection";
    m_isBusy = false;

}
