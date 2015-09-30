#ifndef QFRUITHAPCLIENT_H
#define QFRUITHAPCLIENT_H

#include <QObject>
#include <QJsonDocument>
#include "libs/qamqp/source/qamqpclient.h"
#include "libs/qamqp/source/qamqpexchange.h"
#include "libs/qamqp/source/qamqpqueue.h"



class QFruitHapClient : public QObject
{
    Q_OBJECT
public:
    QFruitHapClient(QString rpcExchangeName, QString rpcRoutingKey, QString pubSubExchangeName, QObject *parent = 0);
    ~QFruitHapClient();
    void setBindingKeys(const QStringList &bindingKeys);
signals:
    void connected();
    void disconnected();
    void rpcQueueReady();
    void responseReceived(const QJsonDocument response);

public slots:
    bool connectToServer(const QString &uri);
    void disconnectFromServer();
    void sendMessage(const QJsonDocument &message);

private slots:
    void clientConnected();
    void rpcQueueDeclared();
    void rpcResponseReceived();
    void pubSubExchangeDeclared();
    void pubSubQueueDeclared();
    void pubSubQueueBound();
    void messageReceived();
private:
    QString m_rpcExchangeName;
    QString m_rpcRoutingKey;
    QString m_pubSubExchangeName;
    QStringList m_bindingKeys;

    QAmqpClient *m_client;
    QAmqpQueue *m_rpcResponseQueue;
    QAmqpExchange *m_rpcExchange;
    QAmqpExchange *m_pubsubExchange;
    QAmqpQueue *m_pubsubQueue;
    QString m_correlationId;


};

#endif // QFRUITHAPCLIENT_H
