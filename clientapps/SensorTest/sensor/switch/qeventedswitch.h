#ifndef QEVENTEDSWITCH_H
#define QEVENTEDSWITCH_H

#include "../qeventedsensor.h"
#include "definitions.h"


class QEventedSwitch : public QEventedSensor
{
    Q_OBJECT
    void sendSignal(const QJsonObject &responseObject);
public:
    QEventedSwitch(QFruitHapClient *client, QString name, QObject *parent = 0);
    void turnOn();
    void turnOff();

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void switchStateReceived(const QString name, SwitchState state);
};

#endif // QEVENTEDSWITCH_H
