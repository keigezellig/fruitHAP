#ifndef QSWITCH_H
#define QSWITCH_H

#include <QObject>

class QSwitch : public QObject
{
    Q_OBJECT
public:
    explicit QSwitch(QObject *parent = 0);

signals:

public slots:
};

#endif // QSWITCH_H
