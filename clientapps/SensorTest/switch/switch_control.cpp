#include "switch_control.h"
#include <algorithm>

SwitchControl::SwitchControl()
{
    m_switches.push_back(Switch("Single"));
    m_switches.push_back(Switch("Double"));
}



void SwitchControl::turnOn(const std::string &sensorName)
{
    std::vector<Switch>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](Switch &sw) {return sw.getName() == sensorName;});
    if (it != m_switches.end())
    {
        it->turnOn();
    }
}

void SwitchControl::turnOff(const std::string &sensorName)
{
    std::vector<Switch>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](Switch &sw) {return sw.getName() == sensorName;});
    if (it != m_switches.end())
    {
        it->turnOff();
    }
}

SwitchState SwitchControl::getState(const std::string &sensorName) const
{
     std::vector<Switch>::const_iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](const Switch &sw) {return sw.getName() == sensorName;});
     if (it != m_switches.end())
     {
        return it->getState();
     }

     return SwitchState::Undefined;

}

std::vector<std::string> SwitchControl::getSwitchNames() const
{
    std::vector<std::string> result;
    for(const Switch& s : m_switches)
    {
        result.push_back(s.getName());
    }

    return result;
}


