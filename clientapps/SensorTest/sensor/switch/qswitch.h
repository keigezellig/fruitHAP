#ifndef QEVENTEDSWITCH_H
#define QEVENTEDSWITCH_H

#include "../qfruithapsensor.h"
#include "definitions.h"


class QSwitch : public QFruitHapSensor
{
    Q_OBJECT
    void sendSignal(const QJsonObject &responseObject);
public:
    QSwitch(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent);
    void turnOn();
    void turnOff();

protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void switchStateReceived(const QString name, SwitchState state);
};

#endif // QEVENTEDSWITCH_H
