#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include <QDateTime>
#include "../qfruithapsensor.h"
#include "../../faceverifier/faceverifier.h"

class QCamera : public QFruitHapSensor
{
    Q_OBJECT
    bool m_isFaceDetectionEnabled;
    FaceVerifier *m_faceVerifier;


    void sendImage(const QJsonObject responseObject);
public:
    QCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, FaceVerifier *faceVerifier, QObject *parent = 0);
    void enableFaceDetection(bool isEnabled);
    bool isFaceDetectionEnabled() const;

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void imageReceived(const QString name, const QByteArray imageData, const QDateTime timestamp);
    void faceDetected(const QString name, const QByteArray imageData, const QDateTime timestamp);
public slots:
};

#endif // QEVENTEDCAMERA_H
