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

import android.app.IntentService;
import android.content.Intent;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.ResultReceiver;
import android.preference.PreferenceManager;
import android.util.Log;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.common.MqProvider;
import com.fruithapnotifier.app.mqproviders.MqProviderFactory;

/**
 * An {@link IntentService} subclass for handling asynchronous task requests in
 * a service on a separate handler thread.
 * <p/>
 */
public class FruithapRpcService extends IntentService
{

    private static final String ACTION_CONFIGURATION = "com.fruithapnotifier.app.service.rpc.action.RPC_CONFIGURATION";
    private static final String ACTION_SENSOR = "com.fruithapnotifier.app.service.rpc.action.RPC_SENSOR";

    private static final String REQUEST = "com.fruithapnotifier.app.service.rpc.parameter.REQUEST";

    private static final String RESULT_RECEIVER = "com.fruithapnotifier.app.service.rpc.parameter.RESULT_RECEIVER" ;
    private static final String CONFIGURATION_ROUTINGKEY = "FruitHAP_RpcQueue.FruitHAP.Core.Action.ConfigurationMessage";
    private static final String CONFIGURATION_MESSAGETYPE = "FruitHAP.Core.Action.ConfigurationMessage:FruitHAP.Core";
    private static final String SENSOR_ROUTINGKEY = "FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage" ;
    private static final String SENSOR_MESSAGETYPE = "FruitHAP.Core.Action.SensorMessage:FruitHAP.Core";
    private static final String LOGTAG = "FruithapRpcService";
    private SharedPreferences preferences;

    /**
     * Starts this service to perform a configuration request with the given parameters. If
     * the service is already performing a task this action will be queued.
     *
     * @see IntentService
     */
    // TODO: Customize helper method
    public static void executeConfigurationRequest(Context context, String request, ResultReceiver resultReceiver)
    {
        Intent intent = new Intent(context, FruithapRpcService.class);
        intent.setAction(ACTION_CONFIGURATION);
        intent.putExtra(REQUEST, request);
        intent.putExtra(RESULT_RECEIVER,resultReceiver);
        context.startService(intent);
    }

    /**
     * Starts this service to perform a sensor request with the given parameters. If
     * the service is already performing a task this action will be queued.
     *
     * @see IntentService
     */
    public static void executeSensorRequest(Context context, String request, ResultReceiver resultReceiver)
    {
        Intent intent = new Intent(context, FruithapRpcService.class);
        intent.setAction(ACTION_SENSOR);
        intent.putExtra(REQUEST, request);
        intent.putExtra(RESULT_RECEIVER,resultReceiver);
        context.startService(intent);
    }

    private ConnectionParameters getConnectionParameters()
    {
        return new ConnectionParameters(preferences.getString("pref_server_hostname","192.168.1.81"),
                Integer.valueOf(preferences.getString("pref_server_port","5672")),
                preferences.getString("pref_server_username","admin"),
                preferences.getString("pref_server_password","admin"),
                preferences.getString("pref_server_vhost","/"),
                preferences.getString("pref_server_rpc_exchange","FruitHAP_RpcExchange"));
    }

    public FruithapRpcService()
    {
        super("FruithapRpcService");
    }

    @Override
    public void onCreate()
    {
        super.onCreate();
        preferences = PreferenceManager.getDefaultSharedPreferences(this);
    }

    @Override
    protected void onHandleIntent(Intent intent)
    {
        if (intent != null)
        {
            final String request = intent.getStringExtra(REQUEST);
            final ConnectionParameters connectionParameters = getConnectionParameters();
            final ResultReceiver resultReceiver = intent.getParcelableExtra(RESULT_RECEIVER);
            String result = null;

            final String action = intent.getAction();
            if (ACTION_CONFIGURATION.equals(action))
            {
                result = sendRequest(request,connectionParameters,CONFIGURATION_ROUTINGKEY, CONFIGURATION_MESSAGETYPE);
            }
            else if (ACTION_SENSOR.equals(action))
            {
                result = sendRequest(request, connectionParameters, SENSOR_ROUTINGKEY, SENSOR_MESSAGETYPE);
            }


            if (result != null)
            {
                Bundle resultBundle = new Bundle();
                resultBundle.putString(Constants.RPC_RESULTDATA, result);
                resultReceiver.send(Constants.RPC_REQUEST_OK,resultBundle);
            }
            else
            {
                resultReceiver.send(Constants.RPC_REQUEST_FAILED,null);
            }

        }
    }


    private String sendRequest(String request, ConnectionParameters connectParameters, String routingKey, String messageType)
    {
        String result = null;
        final MqProvider mqProvider = MqProviderFactory.getMqProviderInstance();
        String host = connectParameters.getHost();
        int port = connectParameters.getPort();
        String username = connectParameters.getUserName();
        String password = connectParameters.getPassword();
        String vhost = connectParameters.getvHost();
        String rpcExchange = connectParameters.getRpcExchange();
        try
        {
            mqProvider.initialize(host, port, username, password, vhost);
            mqProvider.setRpcExchange(rpcExchange);
            result = mqProvider.sendRequest(request, routingKey, messageType);
        }
        catch (Exception ex)
        {
            Log.e(LOGTAG, "Uh oh, something went wrong while executing RPC request", ex);
        }

        return result;
    }



}
