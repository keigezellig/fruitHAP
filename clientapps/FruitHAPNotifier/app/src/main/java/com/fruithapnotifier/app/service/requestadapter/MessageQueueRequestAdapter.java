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

package com.fruithapnotifier.app.service.requestadapter;

import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.ResultReceiver;
import android.util.Log;
import com.fruithapnotifier.app.common.ConfigurationEvent;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.service.FruithapRpcService;
import com.fruithapnotifier.app.service.requestadapter.requests.ConfigurationRequest;
import com.fruithapnotifier.app.service.requestadapter.requests.SensorRequest;
import org.greenrobot.eventbus.EventBus;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Map;

public class MessageQueueRequestAdapter implements RequestAdapter
{
    private static final String LOGTAG = MessageQueueRequestAdapter.class.getName() ;
    private final ResultReceiver sensorRequestReceiver;
    private final ResultReceiver configurationRequestReceiver;
    private Context context;

    public MessageQueueRequestAdapter(Context context)
    {
        this.context = context;
        sensorRequestReceiver = new ResultReceiver(new Handler()) {
            @Override
            protected void onReceiveResult(int resultCode, Bundle resultData)
            {
                if (resultCode == Constants.RPC_REQUEST_OK)
                {
                    String response = resultData.getString(Constants.RPC_RESULTDATA);
                    try
                    {
                        JSONObject eventData = new JSONObject(response);
                        SensorEvent sensorEvent = new SensorEvent(eventData);
                        EventBus.getDefault().post(sensorEvent);

                    }
                    catch (JSONException e)
                    {
                        Log.e(LOGTAG, "Message error", e);
                    }
                }
            }
        };

        configurationRequestReceiver = new ResultReceiver(new Handler()) {
            @Override
            protected void onReceiveResult(int resultCode, Bundle resultData)
            {
                if (resultCode == Constants.RPC_REQUEST_OK)
                {
                    String response = resultData.getString(Constants.RPC_RESULTDATA);
                    try
                    {
                        JSONObject configurationData = new JSONObject(response);
                        ConfigurationEvent configurationEvent = new ConfigurationEvent(configurationData);
                        EventBus.getDefault().post(configurationEvent);

                    }
                    catch (JSONException e)
                    {
                        Log.e(LOGTAG, "Message error", e);
                    }
                }
            }
        };



    }

    @Override
    public void sendSensorRequest(String sensorName, String operationName, Map<String, String> parameters)
    {
        try
        {
            SensorRequest request = new SensorRequest(sensorName, operationName, parameters);
            FruithapRpcService.executeSensorRequest(context, request.toJson().toString(), sensorRequestReceiver);
        }
        catch (JSONException jex)
        {
            Log.e(LOGTAG, "Error in request", jex);
        }

    }

    @Override
    public void sendConfigurationRequest(String operationName, Map<String, String> parameters)
    {
        try
        {
            ConfigurationRequest request = new ConfigurationRequest(operationName, parameters);
            FruithapRpcService.executeConfigurationRequest(context, request.toJson().toString(), configurationRequestReceiver);
        }
        catch (JSONException jex)
        {
            Log.e(LOGTAG, "Error in request", jex);
        }
    }
}
