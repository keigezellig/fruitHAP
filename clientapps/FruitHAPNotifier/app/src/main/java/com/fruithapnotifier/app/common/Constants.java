package com.fruithapnotifier.app.common;

/**
 * Created by developer on 12/5/15.
 */
public class Constants
{
    public static final String RPC_RESULTDATA = "com.fruithapnotifier.app.service.rpc.RESULT_DATA";
    public static final int RPC_REQUEST_OK = 0;
    public static final int RPC_REQUEST_FAILED = -1;
    public static final java.lang.String MQHOST = "com.fruithapnotifier.app.service.rpc.connection.MQHOST" ;
    public static final java.lang.String MQPORT = "com.fruithapnotifier.app.service.rpc.connection.MQPORT";
    public static final java.lang.String MQUSERNAME = "com.fruithapnotifier.app.service.rpc.connection.MQUSERNAME";
    public static final java.lang.String MQPASSWORD = "com.fruithapnotifier.app.service.rpc.connection.MQPASSWORD";
    public static final java.lang.String MQVHOST = "com.fruithapnotifier.app.service.rpc.connection.MQVHOST";
    public static final java.lang.String MQ_RPCEXCHANGE = "com.fruithapnotifier.app.service.rpc.connection.MQ_RPCEXCHANGE" ;
    public static final String MQ_PUBSUB_TOPIC = "com.fruithapnotifier.app.service.pubsub.MQ_PUBSUB_TOPIC";
    public static final String MQ_PUBSUB_MESSAGE = "com.fruithapnotifier.app.service.pubsub.MQ_PUBSUB_MESSAGE";
    public static String START_ACTION = "com.fruithapnotifier.app.action.START_SERVICE";
    public static String STOP_ACTION = "com.fruithapnotifier.app.action.STOP_SERVICE";
    public static String FRUITHAP_NOTIFICATION_ACTION = "com.fruithapnotifier.app.action.FRUITHAP_NOTIFICATION";
    public static int SERVICE_STATE_NOTIFICATIONID = 1;
    public static int INCOMING_EVENT_NOTIFICATIONID = 2;

    public enum MainScreenSection
    {
        ALERT_LIST,
        DASHBOARD
    }
}
