#include "switch.h"


Switch::Switch(const std::string &name): m_name(name), m_state(SwitchState::Undefined)
{
}

void Switch::turnOn()
{
    m_state = SwitchState::On;
}

void Switch::turnOff()
{
    m_state = SwitchState::Off;
}

std::string Switch::getName() const
{
    return m_name;
}

SwitchState Switch::getState() const
{
    return m_state;
}
