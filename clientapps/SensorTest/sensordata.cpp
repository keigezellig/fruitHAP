#include "sensordata.h"



SensorData::SensorData(const QString name, const QString category):
    m_name(name), m_category(category)
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
