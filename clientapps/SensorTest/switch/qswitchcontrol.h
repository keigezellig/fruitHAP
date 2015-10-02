#ifndef QSWITCHCONTROL_H
#define QSWITCHCONTROL_H

#include <QObject>
#include <memory>
#include <QJsonDocument>
#include <vector>
#include <QTimer>
#include "qfruithapclient.h"

enum class SwitchState
{
    Undefined, On, Off
};

class QSwitchControl : public QObject
{
    Q_OBJECT
public:
    QSwitchControl(QFruitHapClient &client,QObject *parent = 0);
    void turnOn(const QString &name);
    void turnOff(const QString &name);    
    void getState (const QString &name);
    void getNames();
    void handleSensorMessage(QJsonObject responseObject);

private:
    bool m_isBusy;    
    QString m_currentName;
    SwitchState m_state;
    QFruitHapClient &m_client;
    QTimer *m_requestTimer;
signals:
    void switchStateReceived(const QString name, SwitchState state);
    void switchListReceived(const QStringList list);
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void requestTimeout();

};

#endif // QSWITCHCONTROL_H
