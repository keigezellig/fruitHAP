#ifndef SWITCH_H
#define SWITCH_H

#include <string>
#include <QObject>


enum class SwitchState
{
    Undefined, On, Off
};

class Switch : public QObject
{
     Q_OBJECT

public:    
    explicit Switch(const std::string &name);
    void turnOn();
    void turnOff();
    std::string getName() const;
    SwitchState getState() const;
private:
    std::string m_name;
    SwitchState m_state;    
signals:
    void stateChanged(const std::string switchName, const SwitchState newState);

};

#endif // SWITCH_H
