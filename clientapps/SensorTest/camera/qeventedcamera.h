#ifndef QEVENTEDCAMERA_H
#define QEVENTEDCAMERA_H

#include <QObject>
#include "qeventedsensor.h"

class QEventedCamera : public QEventedSensor
{
    Q_OBJECT

private:
    QString m_name;

public:
    QEventedCamera(QString m_name, );

signals:

public slots:
};

#endif // QEVENTEDCAMERA_H
