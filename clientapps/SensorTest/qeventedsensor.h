#ifndef QEVENTEDSENSOR_H
#define QEVENTEDSENSOR_H

#include <QObject>
#include "qfruithapclient.h"

class QEventedSensor : public QObject
{
    Q_OBJECT

public:
    QString getName() const;
protected:
    QEventedSensor(QFruitHapClient *client, QString name, QObject *parent = 0);
    QFruitHapClient* m_client;
    QString m_name;
    virtual handleSensorEvent(QJsonObject responseObject) = 0;
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
};

#endif // QEVENTEDSENSOR_H
