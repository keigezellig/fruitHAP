#ifndef SWITCH_H
#define SWITCH_H

#include <string>

enum class SwitchState
{
    Undefined, On, Off
};

class Switch
{
public:
    explicit Switch(const std::string &name);
    void turnOn();
    void turnOff();
    std::string getName() const;
    SwitchState getState() const;
private:
    std::string m_name;
    SwitchState m_state;
};

#endif // SWITCH_H
