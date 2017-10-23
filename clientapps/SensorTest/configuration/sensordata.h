#ifndef SENSORDATA_H
#define SENSORDATA_H
#include <QString>

class SensorData
{
public:
    SensorData(const QString name, const QString category, const QString type, const QString valueType, bool isReadOnly);
    QString getName() const;
    QString getCategory() const;
    QString getType() const;
    QString getValueType() const;
    bool IsReadOnly() const;
private:
    QString m_name;
    QString m_valueType;
    QString m_category;
    QString m_type;
    bool m_isReadOnly;
};

#endif // SENSORDATA_H
