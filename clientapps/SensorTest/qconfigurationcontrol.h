#ifndef QCONFIGURATIONCONTROL_H
#define QCONFIGURATIONCONTROL_H

#include <QObject>

class QConfigurationControl : public QObject
{
    Q_OBJECT
public:
    explicit QConfigurationControl(QObject *parent = 0);

signals:

public slots:
};

#endif // QCONFIGURATIONCONTROL_H
