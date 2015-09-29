#ifndef QSWITCHCONTROL_H
#define QSWITCHCONTROL_H

#include <QObject>

class QSwitchControl : public QObject
{
    Q_OBJECT
public:
    explicit QSwitchControl(QObject *parent = 0);
    void init();
    void turnOn(const QString &switchName);
    void turnOff(const QString &switchName);
    SwitchState getState(const QString &sensorName) const;
    std::vector<QString> getSwitchNames() const;
    ~QSwitchControl();
signals:
    void stateChanged(QString switchName, SwitchState newState);
private slots:
    void switchStateChanged(const QString switchName, const SwitchState newState);
private:
    std::vector<QSwitch> m_switches;
    QFruitHapClient m_client;


};

#endif // QSWITCHCONTROL_H
