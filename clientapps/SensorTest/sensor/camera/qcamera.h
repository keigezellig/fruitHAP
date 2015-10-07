#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include "../qsensor.h"

class QCamera : public QSensor
{
    Q_OBJECT

public:
    QCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent = 0);

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void imageReceived(const QString name, const QByteArray imageData);
public slots:
};

#endif // QEVENTEDCAMERA_H
