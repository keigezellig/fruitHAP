package com.fruithapnotifier.app.service;

import android.content.Context;
import android.util.Log;
import com.rabbitmq.client.*;

import java.net.URISyntaxException;
import java.security.KeyManagementException;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by developer on 12/5/15.
 */
public class RabbitMQNotificationTask extends EventNotificationTask
{

    private ConnectionFactory connectionFactory;
    private String exchangeName;
    private List<String> topics;
    private QueueingConsumer consumer;

    public RabbitMQNotificationTask(Context ctx)
    {
        super(ctx);
        LOGTAG = "RabbitMQNotificationTask";
    }


    @Override
    protected void initialize(String[] parameters) throws Exception
    {
        String amqpUri = parameters[0];
        String exchangeName = parameters[1];

        ArrayList<String> topics = new ArrayList<String>();
        for (int i = 2; i < parameters.length; i++)
        {
            topics.add(parameters[i]);
        }

        setParameters(amqpUri, exchangeName, topics);

        Connection connection = connectionFactory.newConnection();
        Channel channel = connection.createChannel();
        channel.basicQos(1);
        channel.exchangeDeclare(exchangeName, "topic");
        String queueName = channel.queueDeclare().getQueue();
        for (String bindingKey : topics)
        {
            channel.queueBind(queueName, exchangeName, bindingKey);
        }

        consumer = new QueueingConsumer(channel);
        channel.basicConsume(queueName, true, consumer);
    }


    @Override
    protected void doWork(String[] parameters) throws Exception
    {
        try
        {
            Log.d(LOGTAG, "Waiting for messages...");
            QueueingConsumer.Delivery delivery = consumer.nextDelivery();
            String message = new String(delivery.getBody());
            Log.d(LOGTAG, "Message received from quueue: " + message);
        }
        catch (InterruptedException e)
        {
            Log.w(LOGTAG, "Thread was interrupted. This probably means task was cancelled");
        }

    }

    @Override
    protected void cleanup(String[] parameters) throws Exception
    {
        consumer.getChannel().getConnection().close();
    }

    public void setParameters( String amqpUri, String exchangeName, List<String> topics)
    {
        if (amqpUri == null || amqpUri.isEmpty())
        {
            throw new IllegalArgumentException("AMQP URI must be specified");
        }

        if (exchangeName == null || exchangeName.isEmpty())
        {
            throw new IllegalArgumentException("Exchange name must be specified");
        }

        connectionFactory = initializeConnectionFactory(amqpUri);
        this.exchangeName = exchangeName;
        this.topics = topics;
    }

    private ConnectionFactory initializeConnectionFactory(String amqpUri)
    {
        connectionFactory = new ConnectionFactory();
        try
        {
            connectionFactory.setUri(amqpUri);
            connectionFactory.setAutomaticRecoveryEnabled(false);
            connectionFactory.setTopologyRecoveryEnabled(false);
        }
        catch (KeyManagementException | NoSuchAlgorithmException | URISyntaxException e)
        {
            Log.e(LOGTAG, "Cannot initalize RabbitMQ.", e);
        }

        return connectionFactory;
    }


}
