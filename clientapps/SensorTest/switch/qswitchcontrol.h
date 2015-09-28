#ifndef QSWITCHCONTROL_H
#define QSWITCHCONTROL_H

#include <QObject>
#include <memory>
#include <QJsonDocument>
#include <vector>
#include "qfruithapclient.h"

enum class SwitchState
{
    Undefined, On, Off
};

class QSwitchControl : public QObject
{
    Q_OBJECT
public:
    QSwitchControl(std::shared_ptr<QFruitHapClient> client,QObject *parent = 0);
    void turnOn(const QString &name);
    void turnOff(const QString &name);
    void connect(const QString &uri);
    SwitchState getState (const QString &name);
    void getNames(std::vector<QString> &list);
private:
    bool m_isBusy;
    bool m_isConnected;
    SwitchState m_state;
    std::shared_ptr<QFruitHapClient> m_client;
private slots:
    void onClientResponseReceived(const QJsonDocument response);

};

#endif // QSWITCHCONTROL_H
