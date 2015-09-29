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
    QFruitHapClient(QString exchangeName, QString routingKey, QObject *parent = 0);
    ~QFruitHapClient();
signals:
    void connected();
    void responseReceived(const QJsonDocument response);

public slots:
    bool connectToServer(const QString &uri);
    void sendMessage(const QJsonDocument &message);

private slots:
    void clientConnected();
    void queueDeclared();
    void responseFromMQReceived();

private:
    QString m_exchangeName;
    QString m_routingKey;

    QAmqpClient *m_client;
    QAmqpQueue *m_responseQueue;
    QAmqpExchange *m_defaultExchange;
    QString m_correlationId;


};

#endif // QFRUITHAPCLIENT_H
