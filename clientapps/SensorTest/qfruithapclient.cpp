#include "qfruithapclient.h"
#include <QEventLoop>
#include <QUuid>
#include <QDebug>

QFruitHapClient::QFruitHapClient(QString rpcExchangeName, QString rpcRoutingKey, QString pubSubExchangeName, QObject *parent):
    QObject(parent),
    m_rpcExchangeName(rpcExchangeName),
    m_rpcRoutingKey(rpcRoutingKey),
    m_pubSubExchangeName(pubSubExchangeName),
    m_client(0),
    m_rpcResponseQueue(0),
    m_rpcExchange(0),
    m_pubsubExchange(0),
    m_pubsubQueue(0)
{
  m_client = new QAmqpClient(this);
  connect(m_client, SIGNAL(connected()), this, SLOT(clientConnected()));

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

void QFruitHapClient::setBindingKeys(const QStringList &bindingKeys)
{
    m_bindingKeys = bindingKeys;
}

bool QFruitHapClient::connectToServer(const QString &uri)
{
    QEventLoop loop;    
    connect(this, SIGNAL(connected()), &loop, SLOT(quit()));
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
    m_client->disconnectFromHost();
}

void QFruitHapClient::sendMessage(const QJsonDocument &message)
{
    qDebug() << "Sending message";
    m_correlationId = QUuid::createUuid().toString();
    QAmqpMessage::PropertyHash properties;
    properties.insert(QAmqpMessage::ReplyTo, m_rpcResponseQueue->name());
    properties.insert(QAmqpMessage::CorrelationId, m_correlationId);
    properties.insert(QAmqpMessage::Type, "FruitHap.Core.Action.SensorMessage:FruitHAP.Core");

    m_rpcExchange->publish(message.toJson(), m_rpcRoutingKey, properties);
}

void QFruitHapClient::clientConnected()
{
    qDebug() << "QFruitHapClient::clientConnected| Connected to MQ server";
    m_rpcResponseQueue = m_client->createQueue();
    connect(m_rpcResponseQueue, SIGNAL(declared()), this, SLOT(rpcQueueDeclared()));
    connect(m_rpcResponseQueue, SIGNAL(messageReceived()), this, SLOT(rpcResponseReceived()));
    m_rpcResponseQueue->declare(QAmqpQueue::Exclusive | QAmqpQueue::AutoDelete);
    m_rpcExchange = m_client->createExchange(m_rpcExchangeName);


    m_pubsubExchange = m_client->createExchange(m_pubSubExchangeName);
    connect(m_pubsubExchange, SIGNAL(declared()), this, SLOT(pubSubExchangeDeclared()));
    m_pubsubExchange->declare(QAmqpExchange::Topic);
}

void QFruitHapClient::pubSubExchangeDeclared()
{
    m_pubsubQueue = m_client->createQueue();
    connect(m_pubsubQueue, SIGNAL(declared()), this, SLOT(pubSubQueueDeclared()));
    connect(m_pubsubQueue, SIGNAL(bound()), this, SLOT(pubSubQueueBound()));
    connect(m_pubsubQueue, SIGNAL(messageReceived()), this, SLOT(messageReceived()));
    m_pubsubQueue->declare(QAmqpQueue::Exclusive);
}

void QFruitHapClient::pubSubQueueDeclared() {

    foreach (QString bindingKey, m_bindingKeys)
    {
        m_pubsubQueue->bind(m_pubsubExchange, bindingKey);
        qDebug() << " [*] Waiting for incoming messages with binding key " << bindingKey;
    }
}

void QFruitHapClient::pubSubQueueBound() {

    m_pubsubQueue->consume(QAmqpQueue::coNoAck);
}

void QFruitHapClient::messageReceived() {

    QJsonDocument decodedMessage;

    QAmqpMessage message = m_pubsubQueue->dequeue();

    if (!message.payload().isEmpty())
    {
       decodedMessage = QJsonDocument::fromJson(message.payload());
    }

     emit responseReceived(decodedMessage);

}



void QFruitHapClient::rpcQueueDeclared()
{
    m_rpcResponseQueue->consume();
    emit connected();
}


void QFruitHapClient::rpcResponseReceived()
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

     emit responseReceived(decodedMessage);
}

