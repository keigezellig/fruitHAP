package com.fruithapnotifier.app.common;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;
import java.util.concurrent.TimeoutException;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public interface MqProvider
{
    void initialize(String amqpUri);
    void initialize(String host, int port, String username, String password, String vpath);

    void subscribe(ArrayList<String> topics, final MessageCallback receiver) throws Exception;
    void processPublishedMessages() throws IOException, TimeoutException, JSONException;
    void close() throws Exception;
    String sendRequest(String request, String routingKey, String messageType) throws Exception;

    void setRpcExchange(String rpcExchange);

    void setPubSubExchange(String pubSubExchange);
}
