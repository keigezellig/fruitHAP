
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

package com.fruithapnotifier.app.service;

import java.util.List;


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
