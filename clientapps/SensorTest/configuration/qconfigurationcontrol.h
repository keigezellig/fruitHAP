#ifndef QCONFIGURATIONCONTROL_H
#define QCONFIGURATIONCONTROL_H

#include <QJsonObject>
#include <QJsonArray>

#include "../apimanager/fruithapapi.h"
#include "../sensor/qfruithapsensor.h"
#include "../sensor/switch/qswitch.h"
#include "../sensor/camera/qcamera.h"
#include "../mqclient/qfruithapclient.h"
#include "sensordata.h"
#include "../faceverifier/faceverifier.h"
#include <QTimer>


class QConfigurationControl : public QObject
{
    Q_OBJECT
public:
    QConfigurationControl(QFruitHapClient *mqClient, FruitHapApi *apiClient, FaceVerifier *faceVerifier,  QObject *parent = 0);
    void requestSensorList();
    void getAllSensors(QList<QFruitHapSensor*> &list) const;
    QFruitHapSensor* getSensorByName(const QString& name) const;
    void getAllCameras(QList<QCamera*> &list) const;
    void getAllSwitches(QList<QSwitch*> &list) const;
    void coupleFaceDetectionToSwitch(const QString& cameraName, const QString& switchName);
    ~QConfigurationControl();

private:
    FruitHapApi *m_apiClient;
    QFruitHapClient *m_mqClient;
    QList<QFruitHapSensor*> m_sensors;
    FaceVerifier *m_faceVerifier;
    void handleConfigurationMessage(QJsonArray responseObject);
signals:
    void sensorListReceived(const QList<SensorData> list);
    void sensorListLoaded();
private slots:
    void onClientResponseReceived(const QJsonDocument response);
    void onSensorListReceived(const QList<SensorData> list);

};

#endif // QCONFIGURATIONCONTROL_H
