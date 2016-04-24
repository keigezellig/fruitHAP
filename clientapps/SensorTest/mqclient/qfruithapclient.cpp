#include "qfruithapclient.h"
#include <QEventLoop>
#include <QUuid>
#include <QDebug>

QFruitHapClient::QFruitHapClient(QString rpcExchangeName, QString pubSubExchangeName, QObject *parent):
    QObject(parent),
    m_rpcExchangeName(rpcExchangeName),
    m_pubSubExchangeName(pubSubExchangeName),
    m_client(0),
    m_rpcResponseQueue(0),
    m_rpcExchange(0),
    m_pubsubExchange(0),
    m_pubsubQueue(0),
    m_isBusy(false)
{
  m_client = new QAmqpClient(this);
  connect(m_client, &QAmqpClient::connected, this, &QFruitHapClient::onClientConnected);
  connect(m_client, &QAmqpClient::disconnected, this, &QFruitHapClient::onClientDisconnected);
  //connect(m_client, SIGNAL(socketError()), this, SLOT(onAMQPError()));
  //connect(m_client, SIGNAL(error()), this, SLOT(onAMQPError());

  m_requestTimer = new QTimer(this);
  m_requestTimer->setSingleShot(true);
  connect(m_requestTimer,&QTimer::timeout, this, &QFruitHapClient::onRequestTimeout);


}

QFruitHapClient::~QFruitHapClient()
{
    if (m_rpcExchange != nullptr)
    {
        delete m_rpcExchange;
    }
    if (m_rpcResponseQueue != nullptr)
    {
        delete m_rpcResponseQueue;
    }

    if (m_pubsubExchange != nullptr)
    {
        delete m_pubsubExchange;
    }
    if (m_pubsubQueue != nullptr)
    {
        delete m_pubsubQueue;
    }

    if (m_client != nullptr)
    {
        if (m_client->isConnected())
        {
            m_client->disconnectFromHost();
        }
        delete m_client;
    }

}

void QFruitHapClient::setPubSubTopics(const QStringList &topics)
{
    m_pubSubTopics = topics;
}

QStringList QFruitHapClient::
PubSubTopics() const
{
    return m_pubSubTopics;
}

bool QFruitHapClient::connectToServer(const QString &uri)
{
    if (m_client->isConnected())
    {
        disconnectFromServer();
    }

    QEventLoop loop;
    connect(m_client, SIGNAL(connected()), &loop, SLOT(quit()));
    connect(m_client, SIGNAL(socketError(QAbstractSocket::SocketError)), &loop, SLOT(quit()));

    if (uri.isNull() || uri.isEmpty() )
    {
        m_client->connectToHost();
    }
    else
    {
        m_client->connectToHost(uri);
    }
    loop.exec();

    return m_client->isConnected();
}

void QFruitHapClient::disconnectFromServer()
{
    qDebug() << "Disconnect from server";
    m_rpcResponseQueue->close();
    m_pubsubQueue->close();
    m_rpcExchange->close();
    m_pubsubExchange->close();

    delete m_rpcExchange;
    delete m_rpcResponseQueue;
    delete m_pubsubExchange;
    delete m_pubsubQueue;
    m_rpcExchange = nullptr;
    m_rpcResponseQueue = nullptr;
    m_pubsubExchange = nullptr;
    m_pubsubQueue = nullptr;

    m_client->disconnectFromHost();

}

void QFruitHapClient::sendMessage(const QJsonObject &request, const QString &routingKey, const QString &messageType)
{
    if (!m_client->isConnected())
    {
        qCritical() << this->metaObject()->className() << "::Cannot send message, not connected";
        return;
    }

    if (m_isBusy)
    {
       qCritical() << this->metaObject()->className() << "::SendRequest: Busy";
        return;
    }

    qDebug() << this->metaObject()->className() << "::Sending message";
    m_requestTimer->start(4000);
    m_isBusy = true;

    QJsonDocument message(request);

    m_correlationId = QUuid::createUuid().toString();
    QAmqpMessage::PropertyHash properties;
    properties.insert(QAmqpMessage::ReplyTo, m_rpcResponseQueue->name());
    properties.insert(QAmqpMessage::CorrelationId, m_correlationId);
    properties.insert(QAmqpMessage::Type, messageType);

    m_rpcExchange->publish(message.toJson(), routingKey, properties);
}

void QFruitHapClient::onClientConnected()
{
    qDebug() << this->metaObject()->className() << "Connected to MQ server";
    m_rpcResponseQueue = m_client->createQueue();
    connect(m_rpcResponseQueue, SIGNAL(declared()), this, SLOT(onRpcQueueDeclared()));
    connect(m_rpcResponseQueue, SIGNAL(messageReceived()), this, SLOT(onRpcResponseReceived()));
    m_rpcResponseQueue->declare(QAmqpQueue::Exclusive | QAmqpQueue::AutoDelete);
    m_rpcExchange = m_client->createExchange(m_rpcExchangeName);


    m_pubsubExchange = m_client->createExchange(m_pubSubExchangeName);
    connect(m_pubsubExchange, SIGNAL(declared()), this, SLOT(onPubSubExchangeDeclared()));
    m_pubsubExchange->declare(QAmqpExchange::Topic);
}

void QFruitHapClient::onClientDisconnected()
{
    qDebug() << this->metaObject()->className() << "Client disconnected from mq server";
    emit disconnected();
}

void QFruitHapClient::onAMQPError(QAMQP::Error error)
{
    qDebug() << this->metaObject()->className() << "AMQP Error";
    QString errorMessage("AMQP error code: " + error);
}

void QFruitHapClient::onSocketError(QAbstractSocket::SocketError error)
{
    qDebug() << this->metaObject()->className() << "Socket Error";
    QString errorMessage("Connection error: " + error);
}

void QFruitHapClient::onPubSubExchangeDeclared()
{
    m_pubsubQueue = m_client->createQueue();
    connect(m_pubsubQueue, SIGNAL(declared()), this, SLOT(onPubSubQueueDeclared()));
    connect(m_pubsubQueue, SIGNAL(bound()), this, SLOT(onPubSubQueueBound()));
    connect(m_pubsubQueue, SIGNAL(messageReceived()), this, SLOT(onMessageReceived()));
    m_pubsubQueue->declare(QAmqpQueue::Exclusive);
}

void QFruitHapClient::onPubSubQueueDeclared() {

    foreach (QString bindingKey, m_pubSubTopics)
    {
        m_pubsubQueue->bind(m_pubsubExchange, bindingKey);
        qDebug() << " [*] Waiting for incoming messages with binding key " << bindingKey;
        QString uri(m_client->host());
        emit connected(uri);
    }
}

void QFruitHapClient::onPubSubQueueBound() {

    m_pubsubQueue->consume(QAmqpQueue::coNoAck);
}

void QFruitHapClient::onMessageReceived() {

    QJsonDocument decodedMessage;

    QAmqpMessage message = m_pubsubQueue->dequeue();

    if (!message.payload().isEmpty())
    {
       decodedMessage = QJsonDocument::fromJson(message.payload());
    }


     emit responseReceived(decodedMessage, message.routingKey());

}



void QFruitHapClient::onRpcQueueDeclared()
{
    qDebug() << "Rpc queue ready";
    m_rpcResponseQueue->consume();


}


void QFruitHapClient::onRpcResponseReceived()
{
    qDebug() << "QFruitHapClient::responseFromMQReceived| Response received";
    QAmqpMessage message = m_rpcResponseQueue->dequeue();
    QJsonDocument decodedMessage;
    if (message.property(QAmqpMessage::CorrelationId).toString() != m_correlationId)
    {
        // requeue message, it wasn't meant for us
        m_rpcResponseQueue->reject(message, true);
        return;
    }
    if (!message.payload().isEmpty())
    {
       decodedMessage = QJsonDocument::fromJson(message.payload());
    }

    qDebug() << this->metaObject()->className() << "Got answer back";
    m_isBusy = false;
    if (m_requestTimer->isActive())
    {
        m_requestTimer->stop();
    }


     emit responseReceived(decodedMessage,message.property(QAmqpMessage::Type).toString());
}

void QFruitHapClient::onRequestTimeout()
{
    qCritical() << "Request timeout. Check your connection";
    m_isBusy = false;
    emit requestTimedOut();

}

