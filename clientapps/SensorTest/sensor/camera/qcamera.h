#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include <QDateTime>
#include "../qfruithapsensor.h"

class QCamera : public QFruitHapSensor
{
    Q_OBJECT

public:
    QCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent = 0);

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void imageReceived(const QString name, const QByteArray imageData, const QDateTime timestamp);
public slots:
};

#endif // QEVENTEDCAMERA_H
