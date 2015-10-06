#include "sensordata.h"



SensorData::SensorData(const QString name, const QString category, const QString type):
    m_name(name), m_category(category), m_type(type)
{

}

QString SensorData::getName() const
{
    return m_name;
}

QString SensorData::getCategory() const
{
    return m_category;
}

QString SensorData::getType() const
{
    return m_type;
}
