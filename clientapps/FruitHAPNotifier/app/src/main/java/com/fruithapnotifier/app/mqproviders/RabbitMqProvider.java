package com.fruithapnotifier.app.mqproviders;

import android.util.Log;
import com.fruithapnotifier.app.common.MqProvider;
import com.fruithapnotifier.app.common.MessageCallback;
import com.rabbitmq.client.*;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;
import java.util.concurrent.TimeoutException;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public class RabbitMqProvider implements MqProvider
{
    private static final String LOGTAG = "RabbitMqProvider";
    private ConnectionFactory factory;
    private String rpcExchange;
    private String pubSubExchange;
    private Connection connection;
    private Channel channel;
    private QueueingConsumer consumer;
    private MessageCallback receiver;


    @Override
    public void initialize(String amqpUri)
    {

        if (this.factory != null)
        {
            return;
        }

        factory = new ConnectionFactory();

        try
        {
            factory.setUri(amqpUri);
        }
        catch (Exception e)
        {
            Log.e(LOGTAG, "Error setting AMQP uri",e);
            factory = null;
        }



    }
    @Override
    public void initialize(String host, int port, String username, String password, String vpath)
    {
        if (this.factory != null)
        {
            return;
        }

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

    public void subscribe(ArrayList<String> topics, MessageCallback receiver) throws Exception
    {
        if (receiver == null)
        {
            Log.e(LOGTAG,"No receiver specified.. exiting");
            return;
        }

        if (this.pubSubExchange == null || this.pubSubExchange.isEmpty())
        {
            throw new IllegalArgumentException("No PubSub exchange specified");
        }

        this.receiver = receiver;
        if ((connection == null) || (!connection.isOpen()))
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
        Log.i(LOGTAG,"Subscribed.. Waiting for messages");

    }

    public void processPublishedMessages() throws IOException, TimeoutException, JSONException
    {

        try
        {
            QueueingConsumer.Delivery delivery = consumer.nextDelivery();
            String message = new String(delivery.getBody());
            receiver.onMessageReceived(delivery.getEnvelope().getRoutingKey(), message);
        }
        catch (InterruptedException ex)
        {
            Log.w(LOGTAG, "Process was canceled");
            channel.close();
        }
    }

    public void close() throws Exception
    {
        connection.close();
    }

    public String sendRequest(String request, String routingKey, String messageType) throws Exception
    {
        if (this.rpcExchange == null || this.rpcExchange.isEmpty())
        {
            throw new IllegalArgumentException("No RPC exchange specified");
        }

        if ((connection == null) || (!connection.isOpen()))
        {
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

    @Override
    public void setRpcExchange(String rpcExchange)
    {
        this.rpcExchange = rpcExchange;
    }

    @Override
    public void setPubSubExchange(String pubSubExchange)
    {
        this.pubSubExchange = pubSubExchange;
    }
}
