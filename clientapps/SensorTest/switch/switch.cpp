#include "switch.h"



Switch::Switch(const std::string &name): m_name(name), m_state(SwitchState::Undefined)
{
}

void Switch::turnOn()
{
    if (m_state != SwitchState::On)
    {
        m_state = SwitchState::On;
        emit stateChanged(m_name,m_state);
    }
}

void Switch::turnOff()
{
    if (m_state != SwitchState::Off)
    {
        m_state = SwitchState::Off;
        emit stateChanged(m_name,m_state);
    }
}



std::string Switch::getName() const
{
    return m_name;
}

SwitchState Switch::getState() const
{
    return m_state;
}
