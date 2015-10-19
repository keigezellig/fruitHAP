#include "qconfigurationcontrol.h"
#include "facedetectionsettings/facedetectionsettings.h"
#include "facedetectionsettings/facedetectionsettingsmodel.h"


QConfigurationControl::QConfigurationControl(QFruitHapClient *client, FaceVerifier *faceVerifier, QObject *parent):
    QObject(parent), m_client(client), m_faceVerifier(faceVerifier)
{

    connect(m_client,&QFruitHapClient::responseReceived,this,&QConfigurationControl::onClientResponseReceived);
    connect(this,&QConfigurationControl::sensorListReceived,this,&QConfigurationControl::onSensorListReceived);
}


void QConfigurationControl::requestSensorList()
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

void QConfigurationControl::getAllSensors(QList<QFruitHapSensor *> &list) const
{
    list = QList<QFruitHapSensor*>(m_sensors);
}

QFruitHapSensor *QConfigurationControl::getSensorByName(const QString &name) const
{
    foreach (QFruitHapSensor* item, m_sensors)
    {
        if (item->getName() == name)
        {
            return item;
        }
    }

    return nullptr;
}

void QConfigurationControl::getAllCameras(QList<QCamera *> &list) const
{
    list.clear();
    foreach (QFruitHapSensor *sensor, m_sensors)
    {
        QCamera *aCamera = dynamic_cast<QCamera*>(sensor);
        if (aCamera != nullptr)
        {
            list.append(aCamera);
        }
    }

}

void QConfigurationControl::getAllSwitches(QList<QSwitch *> &list) const
{
    list.clear();
    foreach (QFruitHapSensor *sensor, m_sensors)
    {
        QSwitch *aSwitch = dynamic_cast<QSwitch*>(sensor);
        if (aSwitch != nullptr)
        {
            list.append(aSwitch);
        }
    }
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

void QConfigurationControl::onSensorListReceived(const QList<SensorData> list)
{
    qDeleteAll(m_sensors);


    foreach (SensorData item, list)
    {
        if (item.getCategory() == "SwTriggerOnFaceDetectionitch")
        {
            QSwitch *eventedSwitch = new QSwitch(m_client,item.getName(),true,item.IsReadOnly(),parent());
            //connect(eventedSwitch, &QSwitch::errorEventReceived, this, &MainWindow::onErrorReceived);
            m_sensors.append(eventedSwitch);
        }


        if( (item.getType() == "ButtonWithCameraSensor") || (item.getType() == "SwitchWithCameraSensor"))
        {
            QCamera *eventedCamera = new QCamera(m_client,item.getName(),false,item.IsReadOnly(),m_faceVerifier,parent());
            m_sensors.append(eventedCamera);
        }

        if (item.getType() == "Camera")
        {
            QCamera *eventedCamera = new QCamera(m_client,item.getName(),true,item.IsReadOnly(),m_faceVerifier,parent());
            m_sensors.append(eventedCamera);
        }

    }

    QList<FaceDetectionSetting> settings;

    FaceDetectionSettingsModel model;
    model.LoadSettingsFromFile("settings.json",settings);

    foreach (const FaceDetectionSetting &setting, settings)
    {
        coupleFaceDetectionToSwitch(setting.getCameraName(),setting.getSwitchName());
    }


    emit sensorListLoaded();
}

void QConfigurationControl::coupleFaceDetectionToSwitch(const QString& cameraName, const QString& switchName)
{
    QCamera* camera = dynamic_cast<QCamera*>(getSensorByName(cameraName));
    QSwitch* theSwitch = dynamic_cast<QSwitch*>(getSensorByName(switchName));

    if (camera == nullptr)
    {
        qCritical() << "Camera " << cameraName << "not available";
        return;
    }

    if (theSwitch == nullptr)
    {
        qCritical() << "Switch " << switchName << "not available";
        return;
    }

    camera->enableFaceDetection(true);
    connect(camera,&QCamera::faceDetected, theSwitch, &QSwitch::turnOn);
}

QConfigurationControl::~QConfigurationControl()
{
    qDeleteAll(m_sensors);
}
