#include "qswitchcontrol.h"
#include <stdexcept>

QSwitchControl::QSwitchControl(QObject *parent) : m_client(QString("FruitHAP_RpcExchange"),QString("FruitHAP_RpcQueue.FruitHap.Core.Action.SensorMessage"),this), QObject(parent)
{
    setObjectName("switchControl");

    QSwitch switch1("SingleSocket",&m_client,this);
    QSwitch switch2("MultipleSocket",&m_client,this);

    connect(&switch1, &QSwitch::stateChanged, this, &QSwitchControl::switchStateChanged);
    connect(&switch2, &QSwitch::stateChanged, this, &QSwitchControl::switchStateChanged);

    m_switches.push_back(switch1);
    m_switches.push_back(switch2);
}

void QSwitchControl::init()
{
    bool isConnected = m_client.connectToServer(QString("amqp://admin:admin@ljubljana"));
//    if (!isConnected)
//    {
//        throw std::domain_error("Cannot connect");
//    }
}

void QSwitchControl::turnOn(const QString &switchName)
{
    std::vector<QSwitch>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](QSwitch& sw) {return sw.getName() == switchName;});
    if (it != m_switches.end())
    {
        it->turnOn();
    }
}

void QSwitchControl::turnOff(const QString &switchName)
{
    std::vector<QSwitch>::iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](QSwitch sw) {return sw.getName() == switchName;});
    if (it != m_switches.end())
    {
        it->turnOff();
    }
}

SwitchState QSwitchControl::getState(const QString &switchName) const
{
    std::vector<QSwitch>::const_iterator it = std::find_if(m_switches.begin(),m_switches.end(),[&](QSwitch sw) {return sw.getName() == switchName;});
    if (it != m_switches.end())
    {
        it->getState();
    }

    return SwitchState::Undefined;
}

std::vector<QString> QSwitchControl::getSwitchNames() const
{
    std::vector<QString> result;
    for(const QSwitch& s : m_switches)
    {
        result.push_back(s.getName());
    }

    return result;
}

QSwitchControl::~QSwitchControl()
{

}

void QSwitchControl::switchStateChanged(const QString switchName, const SwitchState newState)
{
    emit stateChanged(switchName,newState);
}


/*#include "switch_control.h"
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

}FruitHap.Core.Action

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
}*/
