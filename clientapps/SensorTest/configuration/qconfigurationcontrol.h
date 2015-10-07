#ifndef QCONFIGURATIONCONTROL_H
#define QCONFIGURATIONCONTROL_H

#include <QJsonObject>
#include <QJsonArray>
#include "../mqclient/qfruithapclient.h"
#include "sensordata.h"
#include <QTimer>


class QConfigurationControl : public QObject
{
    Q_OBJECT
public:
    QConfigurationControl(QFruitHapClient *client, QObject *parent = 0);
    void getSensorList();

private:
    QFruitHapClient *m_client;
    void handleConfigurationMessage(QJsonObject responseObject);
signals:
    void sensorListReceived(const QList<SensorData> list);
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);

};

#endif // QCONFIGURATIONCONTROL_H
