#ifndef QSWITCHCONTROL_H
#define QSWITCHCONTROL_H

#include <QObject>
#include <QJsonDocument>
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
    QSwitchControl(QString category, QFruitHapClient &client, QObject *parent = 0);
    void turnOn(const QString &name);
    void turnOff(const QString &name);    
    void getState (const QString &name);


private:
    bool m_isBusy;    
    QString m_currentName;
    QString m_category;
    SwitchState m_state;
    QFruitHapClient &m_client;
    QTimer *m_requestTimer;
    void handleSensorMessage(QJsonObject responseObject);
signals:
    void switchStateReceived(const QString name, SwitchState state);    
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void requestTimeout();


};

#endif // QSWITCHCONTROL_H
