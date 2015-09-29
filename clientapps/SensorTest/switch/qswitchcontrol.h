#ifndef QSWITCHCONTROL_H
#define QSWITCHCONTROL_H

#include <QObject>
#include <memory>
#include <QJsonDocument>
#include <vector>
#include "qfruithaprpcclient.h"

enum class SwitchState
{
    Undefined, On, Off
};

class QSwitchControl : public QObject
{
    Q_OBJECT
public:
    QSwitchControl(QFruitHapRPCClient &client,QObject *parent = 0);
    void turnOn(const QString &name);
    void turnOff(const QString &name);
    void connectToServer(const QString &uri);
    void getState (const QString &name);
    void getNames(std::vector<QString> &list);
private:
    bool m_isBusy;
    bool m_isConnected;
    QString m_currentName;
    SwitchState m_state;
    QFruitHapRPCClient &m_client;
signals:
    void switchStateReceived(const QString name, SwitchState state);
private slots:
    void onClientResponseReceived(const QJsonDocument response);

};

#endif // QSWITCHCONTROL_H
