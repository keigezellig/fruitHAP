#include "qconfigurationcontrol.h"


QConfigurationControl::QConfigurationControl(QFruitHapClient *client, QObject *parent):
    QObject(parent), m_client(client)
{
        connect(m_client,&QFruitHapClient::responseReceived,this,&QConfigurationControl::onClientResponseReceived);
}


void QConfigurationControl::getSensorList()
{
    QJsonObject obj;
    QJsonObject paramObj;

    obj["OperationName"] = "GetAllSensors";
    obj["Parameters"] = paramObj;
    obj["MessageType"] = 0;

    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.ConfigurationMessage");
    QString messageType("FruitHAP.Core.Action.ConfigurationMessage:FruitHAP.Core");
    m_client->sendMessage(obj,routingKey,messageType);
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
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response is empty";
        return;
    }

    QJsonObject responseObject = response.object();
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response object is empty";
        return;
    }

    if (messageType.contains("ConfigurationMessage"))
    {
        handleConfigurationMessage(responseObject);

    }
}

