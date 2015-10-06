#ifndef QEVENTEDSENSOR_H
#define QEVENTEDSENSOR_H

#include <QObject>
#include <QJsonObject>
#include <QTimer>
#include "../mqclient/qfruithapclient.h"

class QEventedSensor : public QObject
{
    Q_OBJECT
    bool m_isBusy;
    QTimer *m_requestTimer;
public:
    QString getName() const;
    bool isPollable() const;
    bool isReadOnly() const;
    void getValue();

protected:
    QEventedSensor(QFruitHapClient *client, QString name, bool isPollable, bool isReadOnly, QObject *parent = 0);
    QFruitHapClient* m_client;
    QString m_name;
    bool m_isPollable;
    bool m_isReadOnly;
    virtual void handleSensorEvent(const QJsonObject responseObject) = 0;
    virtual void handleGetValueEvent(const QJsonObject responseObject);
    void sendRequest(const QJsonObject request);
signals:
    void errorEventReceived(const QString sensorName, const QString msg);
    void responseHandled();

private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void onResponseHandled();
    void onRequestTimeout();
};

#endif // QEVENTEDSENSOR_H
