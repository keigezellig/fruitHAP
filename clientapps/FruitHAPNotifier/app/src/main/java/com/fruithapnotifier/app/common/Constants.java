package com.fruithapnotifier.app.common;

import android.content.Context;
import android.content.Intent;

/**
 * Created by developer on 12/5/15.
 */
public class Constants
{
    public static final String MQ_CONNECTION_PARAMETERS = "com.fruithapnotifier.app.service.messaging.parameter.MQ_CONNECTION_PARAMETERS";
    public static final java.lang.String MQ_HOST = "com.fruithapnotifier.app.service.messaging.connection.MQ_HOST" ;
    public static final java.lang.String MQ_PORT = "com.fruithapnotifier.app.service.messaging.connection.MQ_PORT";
    public static final java.lang.String MQ_USERNAME = "com.fruithapnotifier.app.service.messaging.connection.MQ_USERNAME";
    public static final java.lang.String MQ_PASSWORD = "com.fruithapnotifier.app.service.messaging.connection.MQ_PASSWORD";
    public static final java.lang.String MQ_VHOST = "com.fruithapnotifier.app.service.messaging.connection.MQ_VHOST";
    public static final java.lang.String MQ_RPCEXCHANGE = "com.fruithapnotifier.app.service.messaging.connection.MQ_RPC_EXCHANGE" ;
    public static final java.lang.String MQ_PUBSUBEXCHANGE = "com.fruithapnotifier.app.service.messaging.connection.MQ_PUBSUB_EXCHANGE" ;
    public static final String MQ_PUBSUB_TOPICS_TO_SUBCRIBE = "com.fruithapnotifier.app.service.messaging.connection.MQ_PUBSUB_TOPICS_TO_SUBCRIBE";

    public static final String MQ_PUBSUB_TOPIC = "com.fruithapnotifier.app.service.messaging.MQ_PUBSUB_TOPIC";
    public static final String MQ_PUBSUB_MESSAGE = "com.fruithapnotifier.app.service.messaging.MQ_PUBSUB_MESSAGE";
    public static final String RPC_RESULTDATA = "com.fruithapnotifier.app.service.messaging.RPC_RESULT_DATA";
    public static final int RPC_REQUEST_OK = 0;
    public static final int RPC_REQUEST_FAILED = -1;


    public static final String ALERT_INSERTED = "com.fruithapnotifier.app.action.ALERT_INSERTED";
    public static final String ALERT_DELETED = "com.fruithapnotifier.app.action.ALERT_DELETED";
    public static final String ALERTS_CLEARED = "com.fruithapnotifier.app.action.ALERTS_CLEARED";
    public static final String SERVICE_STOPPED_ACTION = "com.fruithapnotifier.app.action.SERVICE_STOPPED_ACTION";
    public static final String EXPANDED_ALERTID = "com.fruithapnotifier.app.gui.EXPANDED_ALERTID" ;
    public static final String ALERT_UPDATED = "com.fruithapnotifier.app.action.ALERT_UPDATED";
    public static final String ALERTS_RANGEUPDATED = "com.fruithapnotifier.app.action.ALERTS_RANGEUPDATED";
    public static final String ALERT_RANGEDATA = "com.fruithapnotifier.app.data.ALERT_RANGEDATA";
    public static final String ALERT_DATA = "com.fruithapnotifier.app.data.ALERT_DATA";

    public static String STOP_ACTION = "com.fruithapnotifier.app.action.STOP_SERVICE";
    public static String INCOMING_ALERT = "com.fruithapnotifier.app.action.INCOMING_ALERT";
    public static int SERVICE_STATE_NOTIFICATIONID = 1;
    public static int INCOMING_ALERT_NOTIFICATION = 2;

    public enum Section
    {
        ALERT_LIST,
        DASHBOARD
    }

    public class ALERTRANGE
    {
    }
}
