#ifndef QCONFIGURATIONCONTROL_H
#define QCONFIGURATIONCONTROL_H

#include <QJsonObject>
#include <QJsonArray>
#include "qfruithapclient.h"
#include "sensordata.h"
#include <QTimer>


class QConfigurationControl : public QObject
{
    Q_OBJECT
public:
    QConfigurationControl(QFruitHapClient &client, QObject *parent = 0);
    QConfigurationControl(const QConfigurationControl& copy);
    void getSensorNames();

private:
    QFruitHapClient &m_client;
    QString m_category;
    QTimer *m_requestTimer;
    bool m_isBusy;
    void handleConfigurationMessage(QJsonObject responseObject);
signals:
    void sensorListReceived(const QList<SensorData> list);
private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void requestTimeout();

};

#endif // QCONFIGURATIONCONTROL_H
