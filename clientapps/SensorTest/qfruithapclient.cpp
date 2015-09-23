#include "qfruithapclient.h"

QFruitHapClient::QFruitHapClient(QString exchangeName, QString routingKey,QObject *parent):
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


bool QFruitHapClient::connectToServer(const QString uri)
{

}

void QFruitHapClient::sendMessage(const QJsonDocument message)
{

}

void QFruitHapClient::clientConnected()
{

}

void QFruitHapClient::queueDeclared()
{

}

void QFruitHapClient::responseFromMQReceived()
{

}

