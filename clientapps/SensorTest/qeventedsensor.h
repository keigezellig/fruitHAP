#ifndef QEVENTEDSENSOR_H
#define QEVENTEDSENSOR_H

#include <QObject>
#include "qfruithapclient.h"

class QEventedSensor : public QObject
{
    Q_OBJECT

protected:
    QEventedSensor(QFruitHapClient *client, QObject *parent = 0);
    QFruitHapClient* m_client;
    virtual handleSensorMessage(QJsonObject responseObject) = 0;
    ~QEventedSensor();
signals:
    void errorReceived(QString sensorName, QString message);
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
};

#endif // QEVENTEDSENSOR_H
