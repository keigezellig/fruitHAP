#ifndef QSWITCH_H
#define QSWITCH_H

#include <QObject>
#include <QString>
#include <QJsonDocument>

enum class SwitchState
{
    Undefined, On, Off
};

class QSwitch : public QObject
{
    Q_OBJECT
public:    
    QSwitch(const QString name, QObject *parent = 0);
    void turnOn();
    void turnOff();
    QString getName() const;
    SwitchState getState() const;
private:
    QString m_name;
    SwitchState m_state;
signals:
    void stateChanged(const QString switchName, const SwitchState newState);
private slots:
    void onMessageReceived(const QJsonDocument message);
};

#endif // QSWITCH_H
