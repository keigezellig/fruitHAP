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
protected:
    virtual void handleSensorEvent(const QJsonObject responseObject);
    virtual void handleGetValueEvent(const QJsonObject responseObject);

signals:
    void switchStateReceived(const SwitchState state, const QDateTime dateTime);
public slots:
    void turnOn();
    void turnOff();


};

#endif // QEVENTEDSWITCH_H
