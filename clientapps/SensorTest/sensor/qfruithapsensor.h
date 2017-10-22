#ifndef QEVENTEDSENSOR_H
#define QEVENTEDSENSOR_H

#include <QObject>
#include <QJsonObject>
#include <QTimer>
#include "../mqclient/qfruithapclient.h"
#include "../apimanager/fruithapapi.h"

class QFruitHapSensor : public QObject
{
    Q_OBJECT

public:
    QString getName() const;
    bool isPollable() const;
    bool isReadOnly() const;


protected:
    QFruitHapSensor(QFruitHapClient *client, FruitHapApi *apiClient, QString name, bool isPollable, bool isReadOnly, QObject *parent = 0);
    QFruitHapClient* m_client;
    FruitHapApi* m_apiClient;
    QString m_name;
    bool m_isPollable;
    bool m_isReadOnly;
    virtual void handleSensorEvent(const QJsonObject responseObject) = 0;
    virtual void handleGetValueEvent(const QJsonObject responseObject);
    void sendRequest(const QJsonObject request);
signals:
    void errorEventReceived(const QString sensorName, const QString msg);

private slots:
    void onClientResponseReceived(const QJsonDocument response, const QString messageType);
    void onApiResponseReceived(const QJsonDocument response);
public slots:
    void getValue();


};

#endif // QEVENTEDSENSOR_H
