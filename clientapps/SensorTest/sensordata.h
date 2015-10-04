#ifndef SENSORDATA_H
#define SENSORDATA_H
#include <QString>

class SensorData
{
public:
    SensorData(const QString name, const QString category);
    QString getName() const;
    QString getCategory() const;
private:
    QString m_name;
    QString m_category;
};

#endif // SENSORDATA_H
