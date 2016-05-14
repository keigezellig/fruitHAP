package com.myown.rabbitmqtest.common;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public interface IMQProvider
{
    void subscribe(String[] topics) throws Exception;

    void processMessages(IMQSubscriptionReceiver receiver) throws IOException, TimeoutException;

    void close() throws Exception;

    String sendRequest(String request, String routingKey, String messageType) throws Exception;
    void publishMessage(String message, String routingKey) throws IOException, TimeoutException;
}
