package com.myown.rabbitmqtest.mqproviders;

import com.myown.rabbitmqtest.common.IMQProvider;
import com.myown.rabbitmqtest.common.IMQSubscriptionReceiver;
import com.rabbitmq.client.*;

import java.io.IOException;
import java.net.URISyntaxException;
import java.security.KeyManagementException;
import java.security.NoSuchAlgorithmException;
import java.util.concurrent.TimeoutException;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public class RabbitMQProvider implements IMQProvider
{
    private final ConnectionFactory factory;
    private String rpcExchange;
    private String pubSubExchange;
    private Connection connection;
    private Channel channel;
    private QueueingConsumer consumer;


    public RabbitMQProvider(String amqpUri, String rpcExchange, String pubSubExchange)
    {
        this.rpcExchange = rpcExchange;
        this.pubSubExchange = pubSubExchange;

        factory = new ConnectionFactory();
        try
        {
            factory.setUri(amqpUri);
        }
        catch (URISyntaxException e)
        {
            e.printStackTrace();
        }
        catch (NoSuchAlgorithmException e)
        {
            e.printStackTrace();
        }
        catch (KeyManagementException e)
        {
            e.printStackTrace();
        }
    }
    public RabbitMQProvider(String host, int port, String username, String password, String vpath, String rpcExchange, String pubSubExchange)
    {
        this.rpcExchange = rpcExchange;
        this.pubSubExchange = pubSubExchange;

        factory = new ConnectionFactory();
        if (host != null && !host.isEmpty())
        {
            factory.setHost(host);
        }

        if (port != 0)
        {
            factory.setPort(port);
        }

        if (username != null && !username.isEmpty())
        {
            factory.setUsername(username);
        }

        if (password != null && !password.isEmpty())
        {
            factory.setPassword(password);
        }

        if (vpath != null && !vpath.isEmpty())
        {
            factory.setVirtualHost(vpath);
        }

    }

    public void subscribe(String[] topics) throws Exception
    {

        if (connection == null)
        {
            connection = factory.newConnection();
        }
        channel = connection.createChannel();

        channel.exchangeDeclare( pubSubExchange, "topic");
        String queueName = channel.queueDeclare().getQueue();

        for (String bindingKey : topics)
        {
            channel.queueBind(queueName, pubSubExchange, bindingKey);
        }

        consumer = new QueueingConsumer(channel);
        channel.basicConsume(queueName, true, consumer);
        System.out.println("Subscribed.. Waiting for messages");

    }

    public void processMessages(final IMQSubscriptionReceiver receiver) throws IOException, TimeoutException {


        try
        {
            QueueingConsumer.Delivery delivery = consumer.nextDelivery();
            String message = new String(delivery.getBody());
            receiver.onMessageReceived(delivery.getEnvelope().getRoutingKey(), message);
        }
        catch (InterruptedException ex)
        {
            System.out.print("Process was canceled");
            channel.close();
        }


    }



    public void close() throws Exception
    {
        connection.close();
    }


    public void publishMessage(String message, String routingKey) throws IOException, TimeoutException
    {
        if (connection == null) {
            connection = factory.newConnection();
        }
        channel = connection.createChannel();

        channel.exchangeDeclare(pubSubExchange, "topic");
        channel.basicPublish(pubSubExchange, routingKey, null, message.getBytes());
        channel.close();
    }



    public String sendRequest(String request, String routingKey, String messageType) throws Exception
    {
        if (connection == null) {
            connection = factory.newConnection();
        }
        Channel channel = connection.createChannel();

        String replyQueueName = channel.queueDeclare().getQueue();
        QueueingConsumer consumer = new QueueingConsumer(channel);
        channel.basicConsume(replyQueueName, true, consumer);
        String response = null;
        String corrId = java.util.UUID.randomUUID().toString();

        AMQP.BasicProperties props = new AMQP.BasicProperties()
                .builder()
                .correlationId(corrId)
                .replyTo(replyQueueName)
                .type(messageType)
                .build();

        channel.basicPublish(rpcExchange, routingKey, props, request.getBytes());

        while (true)
        {
            QueueingConsumer.Delivery delivery = consumer.nextDelivery();
            if (delivery.getProperties().getCorrelationId().equals(corrId)) {
                response = new String(delivery.getBody());
                break;
            }
        }

        channel.close();

        return response;

    }
}
