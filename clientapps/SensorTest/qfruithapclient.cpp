#include "qfruithapclient.h"
#include <QEventLoop>
#include <QUuid>

QFruitHapClient::QFruitHapClient(QString &exchangeName, QString &routingKey, QObject *parent):
    QObject(parent),
    m_exchangeName(exchangeName),
    m_routingKey(routingKey),
    m_client(0),
    m_responseQueue(0),
    m_defaultExchange(0)
{
  m_client = new QAmqpClient(this);
  connect(m_client, SIGNAL(connected()), this, SLOT(clientConnected()));

}


bool QFruitHapClient::connectToServer(const QString &uri)
{
    QEventLoop loop;
    connect(this, SIGNAL(connected()), &loop, SLOT(quit()));
    m_client->connectToHost(uri);
    loop.exec();

    return m_client->isConnected();
}

void QFruitHapClient::disconnectFromServer()
{
    m_client->disconnectFromHost();
}

void QFruitHapClient::sendMessage(const QJsonDocument &message)
{
    m_correlationId = QUuid::createUuid().toString();
    QAmqpMessage::PropertyHash properties;
    properties.insert(QAmqpMessage::ReplyTo, m_responseQueue->name());
    properties.insert(QAmqpMessage::CorrelationId, m_correlationId);
    properties.insert(QAmqpMessage::Type, "FruitHap.StandardActions.Messages.SensorMessage:FruitHap.StandardActions");

    m_defaultExchange->publish(message.toJson(), m_routingKey, properties);
}

void QFruitHapClient::clientConnected()
{
    m_responseQueue = m_client->createQueue();
    connect(m_responseQueue, SIGNAL(declared()), this, SLOT(queueDeclared()));
    connect(m_responseQueue, SIGNAL(messageReceived()), this, SLOT(responseReceived()));
    m_responseQueue->declare(QAmqpQueue::Exclusive | QAmqpQueue::AutoDelete);
    m_defaultExchange = m_client->createExchange(m_exchangeName);
}

void QFruitHapClient::queueDeclared()
{
    m_responseQueue->consume();
    emit connected();
}


void QFruitHapClient::responseFromMQReceived()
{
    QAmqpMessage message = m_responseQueue->dequeue();
    QJsonDocument decodedMessage;
    if (message.property(QAmqpMessage::CorrelationId).toString() != m_correlationId)
    {
        // requeue message, it wasn't meant for us
        m_responseQueue->reject(message, true);
        return;
    }
    if (!message.payload().isEmpty())
    {
       decodedMessage = QJsonDocument::fromJson(message.payload());
    }

     emit responseReceived(decodedMessage);
}

