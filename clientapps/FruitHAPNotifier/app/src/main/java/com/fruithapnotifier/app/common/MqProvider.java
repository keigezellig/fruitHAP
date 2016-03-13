
/*
 *  -----------------------------------------------------------------------------------------------
 *  "THE APPRECIATION LICENSE" (Revision 0x100):
 *  Copyright (c) 2016. M. Joosten
 *  You can do anything with this program and its code, even use it to run a nuclear reactor (why should you)
 *  But I won't be responsible for possible damage and mishap, because i never tested it on a nuclear reactor (why should I..)
 *  If you think this program/code is absolutely great and supercalifragilisticexpialidocious (or just plain useful), just
 *  let me know by sending me a nice email or postcard from your country of origin and leave this header in the code.
 *  Or better join my efforts to make this program even better :-)
 *  See my blog (http://joosten-industries/blog), for contact info
 *  ---------------------------------------------------------------------------------------------------
 *
 *
 */

package com.fruithapnotifier.app.common;

import org.json.JSONException;

import java.io.IOException;
import java.util.List;
import java.util.concurrent.TimeoutException;

public interface MqProvider
{
    void initialize(String amqpUri);
    void initialize(String host, int port, String username, String password, String vpath);

    void subscribe(List<String> topics, final MessageCallback receiver) throws Exception;
    void processPublishedMessages() throws IOException, TimeoutException, JSONException;
    void close() throws Exception;
    String sendRequest(String request, String routingKey, String messageType) throws Exception;

    void setRpcExchange(String rpcExchange);

    void setPubSubExchange(String pubSubExchange);
}
