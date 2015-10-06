#ifndef SENSORDATA_H
#define SENSORDATA_H
#include <QString>

class SensorData
{
public:
    SensorData(const QString name, const QString category, const QString type);
    QString getName() const;
    QString getCategory() const;
    QString getType() const;
private:
    QString m_name;
    QString m_category;
    QString m_type;
};

#endif // SENSORDATA_H
