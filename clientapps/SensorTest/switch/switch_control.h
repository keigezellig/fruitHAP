#ifndef BASICSWITCHCONTROL_H
#define BASICSWITCHCONTROL_H

#include "switch.h"
#include <vector>



class SwitchControl : public QObject
{
    Q_OBJECT
public:
    SwitchControl();
    void turnOn(const std::string &sensorName);
    void turnOff(const std::string &sensorName);
    SwitchState getState(const std::string &sensorName) const;
    std::vector<std::string> getSwitchNames() const;
    ~SwitchControl();
signals:
    void stateChanged(std::string switchName, SwitchState newState);
private slots:
    void switchStateChanged(const std::string switchName, const SwitchState newState);
private:
    std::vector<Switch*> m_switches;


};

#endif // BASICSWITCHCONTROL_H
