package com.fruithapnotifier.app.service;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by MJOX03 on 22.1.2016.
 */
public class ConnectionParameters
{
    private String host;
    private int port;
    private String userName;
    private String password;
    private String vHost;
    private String rpcExchange;
    private String pubSubExchange;
    private List<String> pubSubTopics;


    public ConnectionParameters(String host, int port, String userName, String password, String vHost, String rpcExchange)
    {
        this.host = host;
        this.port = port;
        this.userName = userName;
        this.password = password;
        this.vHost = vHost;
        this.rpcExchange = rpcExchange;
    }

    public ConnectionParameters(String host, int port, String userName, String password, String vHost, String pubSubExchange, List<String> pubSubTopics)
    {
        this.host = host;
        this.port = port;
        this.userName = userName;
        this.password = password;
        this.vHost = vHost;
        this.pubSubExchange = pubSubExchange;
        this.pubSubTopics = pubSubTopics;
    }


    public String getHost()
    {
        return host;
    }

    public int getPort()
    {
        return port;
    }

    public String getUserName()
    {
        return userName;
    }

    public String getPassword()
    {
        return password;
    }

    public String getvHost()
    {
        return vHost;
    }

    public String getRpcExchange()
    {
        return rpcExchange;
    }

    public String getPubSubExchange()
    {
        return pubSubExchange;
    }

    public List<String> getPubSubTopics()
    {
        return pubSubTopics;
    }


    @Override
    public String toString()
    {
        return "ConnectionParameters{" +
                "host='" + host + '\'' +
                ", port=" + port +
                ", userName='" + userName + '\'' +
                ", password='" + password + '\'' +
                ", vHost='" + vHost + '\'' +
                ", rpcExchange='" + rpcExchange + '\'' +
                ", pubSubExchange='" + pubSubExchange + '\'' +
                ", pubSubTopics=" + pubSubTopics +
                '}';
    }
}
