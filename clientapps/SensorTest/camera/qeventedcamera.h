#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include "qeventedsensor.h"

class QEventedCamera : public QEventedSensor
{
    Q_OBJECT

public:
    QEventedCamera(QFruitHapClient *client, QString name, QObject *parent = 0);

protected:
    virtual handleSensorEvent(QJsonObject responseObject);

signals:
    void imageReceived(const QString name, const QByteArray imageData);
public slots:
};

#endif // QEVENTEDCAMERA_H
