
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

import android.content.Context;
import android.content.Intent;

public class Constants
{
    public static final String MQ_CONNECTION_PARAMETERS = "com.fruithapnotifier.app.service.messaging.parameter.MQ_CONNECTION_PARAMETERS";
    public static final java.lang.String MQ_HOST = "com.fruithapnotifier.app.service.messaging.connection.MQ_HOST" ;
    public static final java.lang.String MQ_PORT = "com.fruithapnotifier.app.service.messaging.connection.MQ_PORT";
    public static final java.lang.String MQ_USERNAME = "com.fruithapnotifier.app.service.messaging.connection.MQ_USERNAME";
    public static final java.lang.String MQ_PASSWORD = "com.fruithapnotifier.app.service.messaging.connection.MQ_PASSWORD";
    public static final java.lang.String MQ_VHOST = "com.fruithapnotifier.app.service.messaging.connection.MQ_VHOST";
    public static final java.lang.String MQ_RPCEXCHANGE = "com.fruithapnotifier.app.service.messaging.connection.MQ_RPC_EXCHANGE" ;

    public static final String MQ_PUBSUB_TOPIC = "com.fruithapnotifier.app.service.messaging.MQ_PUBSUB_TOPIC";
    public static final String MQ_PUBSUB_MESSAGE = "com.fruithapnotifier.app.service.messaging.MQ_PUBSUB_MESSAGE";
    public static final String RPC_RESULTDATA = "com.fruithapnotifier.app.service.messaging.RPC_RESULT_DATA";
    public static final int RPC_REQUEST_OK = 0;
    public static final int RPC_REQUEST_FAILED = -1;


    public static final String ALERT_INSERTED = "com.fruithapnotifier.app.action.ALERT_INSERTED";
    public static final String ALERT_DELETED = "com.fruithapnotifier.app.action.ALERT_DELETED";
    public static final String ALERTS_CLEARED = "com.fruithapnotifier.app.action.ALERTS_CLEARED";
    public static final String EXPANDED_ALERTID = "com.fruithapnotifier.app.gui.EXPANDED_ALERTID" ;
    public static final String ALERT_UPDATED = "com.fruithapnotifier.app.action.ALERT_UPDATED";
    public static final String ALERTS_RANGEUPDATED = "com.fruithapnotifier.app.action.ALERTS_RANGEUPDATED";
    public static final String ALERT_RANGEDATA = "com.fruithapnotifier.app.data.ALERT_RANGEDATA";
    public static final String ALERT_DATA = "com.fruithapnotifier.app.data.ALERT_DATA";
    public static final String REST_RESULT = "com.fruithapnotifier.app.service.restclient.REST_RESULT";

    public static String STOP_ACTION = "com.fruithapnotifier.app.action.STOP_SERVICE";
    public static String INCOMING_MESSAGE = "com.fruithapnotifier.app.action.INCOMING_MESSAGE";
    public static int SERVICE_STATE_NOTIFICATIONID = 1;
    public static int INCOMING_ALERT_NOTIFICATION = 2;

    public enum Section
    {
        ALERT_LIST,
        DASHBOARD
    }


}
