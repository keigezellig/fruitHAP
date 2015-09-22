#ifndef BASICSWITCHCONTROL_H
#define BASICSWITCHCONTROL_H

#include "switch.h"
#include <vector>



class SwitchControl
{
public:
    SwitchControl();
    void turnOn(const std::string &sensorName);
    void turnOff(const std::string &sensorName);
    SwitchState getState(const std::string &sensorName) const;
    std::vector<std::string> getSwitchNames() const;
private:
    std::vector<Switch> m_switches;
};

#endif // BASICSWITCHCONTROL_H
