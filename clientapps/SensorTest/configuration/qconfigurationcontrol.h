#ifndef QCONFIGURATIONCONTROL_H
#define QCONFIGURATIONCONTROL_H

#include <QJsonObject>
#include <QJsonArray>
#include "../mqclient/qfruithapclient.h"
#include "../sensor/qfruithapsensor.h"
#include "../sensor/switch/qswitch.h"
#include "../sensor/camera/qcamera.h"
#include "sensordata.h"
#include "../faceverifier/faceverifier.h"
#include <QTimer>


class QConfigurationControl : public QObject
{
    Q_OBJECT
public:
    QConfigurationControl(QFruitHapClient *client, FaceVerifier *faceVerifier,  QObject *parent = 0);
    void requestSensorList();
    void getAllSensors(QList<QFruitHapSensor*> &list) const;
    QFruitHapSensor* getSensorByName(const QString& name) const;
    void getAllCameras(QList<QCamera*> &list) const;
    void getAllSwitches(QList<QSwitch*> &list) const;
    void coupleFaceDetectionToSwitch(const QString& cameraName, const QString& switchName);
    ~QConfigurationControl();

private:
    QFruitHapClient *m_client;
    QList<QFruitHapSensor*> m_sensors;
    FaceVerifier *m_faceVerifier;
    void handleConfigurationMessage(QJsonObject responseObject);
signals:
    void sensorListReceived(const QList<SensorData> list);
    void sensorListLoaded();
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void onSensorListReceived(const QList<SensorData> list);

};

#endif // QCONFIGURATIONCONTROL_H
