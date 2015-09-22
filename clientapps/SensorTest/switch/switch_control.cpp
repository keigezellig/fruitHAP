#include "switch_control.h"
#include <algorithm>

SwitchControl::SwitchControl()
{
    setObjectName("switchControl");
    Switch* switch1 = new Switch("Single");
    Switch* switch2 = new Switch("Double");
    connect(switch1, &Switch::stateChanged, this, &SwitchControl::switchStateChanged);
    connect(switch2, &Switch::stateChanged, this, &SwitchControl::switchStateChanged);

    m_switches.push_back(switch1);
    m_switches.push_back(switch2);

}





void SwitchControl::turnOn(const std::string &sensorName)
{
    std::vector<Switch*>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](Switch* sw) {return sw->getName() == sensorName;});
    if (it != m_switches.end())
    {
        (*it)->turnOn();
    }
}

void SwitchControl::turnOff(const std::string &sensorName)
{
    std::vector<Switch*>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](Switch* sw) {return sw->getName() == sensorName;});
    if (it != m_switches.end())
    {
        (*it)->turnOff();
    }
}

SwitchState SwitchControl::getState(const std::string &sensorName) const
{
     std::vector<Switch*>::const_iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](const Switch* sw) {return sw->getName() == sensorName;});
     if (it != m_switches.end())
     {
        return (*it)->getState();
     }

     return SwitchState::Undefined;

}

std::vector<std::string> SwitchControl::getSwitchNames() const
{
    std::vector<std::string> result;
    for(const Switch* s : m_switches)
    {
        result.push_back(s->getName());
    }

    return result;
}


SwitchControl::~SwitchControl()
{
    for(Switch* s : m_switches)
    {
        delete s;
    }
}

void SwitchControl::switchStateChanged(const std::string switchName, const SwitchState newState)
{
    emit stateChanged(switchName,newState);
}


