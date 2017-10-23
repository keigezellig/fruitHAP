#include "qconfigurationcontrol.h"
#include "facedetectionsettings/facedetectionsettings.h"
#include "facedetectionsettings/facedetectionsettingsmodel.h"



QConfigurationControl::QConfigurationControl(QFruitHapClient *mqClient, FruitHapApi *apiClient, FaceVerifier *faceVerifier, QObject *parent):
    QObject(parent), m_mqClient(mqClient), m_apiClient(apiClient), m_faceVerifier(faceVerifier)
{
    m_sensors = QList<QFruitHapSensor*>();
    connect(m_apiClient,&FruitHapApi::configResponseReceived,this,&QConfigurationControl::onClientResponseReceived);
    connect(this,&QConfigurationControl::sensorListReceived,this,&QConfigurationControl::onSensorListReceived);
}


void QConfigurationControl::requestSensorList()
{
    //TODO: use network manager to do request
    m_apiClient->requestConfiguration();
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

void QConfigurationControl::handleConfigurationMessage(QJsonArray responseObject)
{
        QList<SensorData> sensorDataList;        

        foreach(auto sensor, responseObject)
        {
            QJsonObject sensorObject = sensor.toObject();
            SensorData data(sensorObject["Name"].toString(),sensorObject["Category"].toString(), sensorObject["Type"].toString(), sensorObject["ValueType"].toString(), sensorObject["Type"].toString().contains("ReadOnly") );
            sensorDataList.append(data);

        }

        emit sensorListReceived(sensorDataList);

}

void QConfigurationControl::onClientResponseReceived(const QJsonDocument response)
{
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response is empty";
        return;
    }

    QJsonArray responseObject = response.array();
    if (response.isNull() || response.isEmpty())
    {
        qCritical() << "QCameraControl::onClientResponseReceived| Response object is empty";
        return;
    }


   handleConfigurationMessage(responseObject);


}

void QConfigurationControl::onSensorListReceived(const QList<SensorData> list)
{
    qDebug() << "onSensorListReceived";
    qDeleteAll(m_sensors);


    foreach (SensorData item, list)
    {
        if (item.getValueType() == "OnOffValue")
        {
            QSwitch *eventedSwitch = new QSwitch(m_mqClient, m_apiClient, item.getName(),true,item.IsReadOnly(),parent());
            //connect(eventedSwitch, &QSwitch::errorEventReceived, this, &MainWindow::onErrorReceived);            
            m_sensors.append(eventedSwitch);
            qDebug() << "Added " << item.getName();
        }

        if (item.getValueType() == "ImageValue")
        {
            QCamera *eventedCamera = new QCamera(m_mqClient, m_apiClient, item.getName(),true,item.IsReadOnly(),m_faceVerifier,parent());
            m_sensors.append(eventedCamera);
            qDebug() << "Added " << item.getName();
        }

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
