#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include "../qeventedsensor.h"

class QEventedCamera : public QEventedSensor
{
    Q_OBJECT

public:
    QEventedCamera(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent = 0);

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void imageReceived(const QString name, const QByteArray imageData);
public slots:
};

#endif // QEVENTEDCAMERA_H
