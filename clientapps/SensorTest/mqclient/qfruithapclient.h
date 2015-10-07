#ifndef QFRUITHAPCLIENT_H
#define QFRUITHAPCLIENT_H

#include <QObject>
#include <QJsonDocument>
#include "libs/qamqp/source/qamqpclient.h"
#include "libs/qamqp/source/qamqpexchange.h"
#include "libs/qamqp/source/qamqpqueue.h"
#include <QTimer>



class QFruitHapClient : public QObject
{
    Q_OBJECT
    bool m_isBusy;
    QTimer *m_requestTimer;
public:
    QFruitHapClient(QString rpcExchangeName, QString pubSubExchangeName, QObject *parent = 0);
    ~QFruitHapClient();
    void setPubSubTopics(const QStringList &topics);
    QStringList getPubSubTopics() const;
signals:
    void connected();
    void disconnected();
    void rpcQueueReady();
    void responseReceived(const QJsonDocument response, const QString responseType);

public slots:
    bool connectToServer(const QString &uri);
    void disconnectFromServer();
    void sendMessage(const QJsonObject &message, const QString &routingKey, const QString &messageType);

private slots:
    void clientConnected();
    void rpcQueueDeclared();
    void rpcResponseReceived();
    void pubSubExchangeDeclared();
    void pubSubQueueDeclared();
    void pubSubQueueBound();
    void messageReceived();
    void onRequestTimeout();
private:
    QString m_rpcExchangeName;
    QString m_rpcRoutingKey;
    QString m_pubSubExchangeName;
    QStringList m_pubSubTopics;

    QAmqpClient *m_client;
    QAmqpQueue *m_rpcResponseQueue;
    QAmqpExchange *m_rpcExchange;
    QAmqpExchange *m_pubsubExchange;
    QAmqpQueue *m_pubsubQueue;
    QString m_correlationId;



};

#endif // QFRUITHAPCLIENT_H
