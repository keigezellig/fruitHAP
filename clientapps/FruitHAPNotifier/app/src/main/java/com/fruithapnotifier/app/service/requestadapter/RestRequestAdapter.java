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
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.ResultReceiver;
import android.preference.PreferenceManager;
import android.util.Log;
import android.widget.Toast;
import com.fruithapnotifier.app.common.*;
import com.fruithapnotifier.app.service.RestConsumer;
import com.fruithapnotifier.app.service.requestadapter.requests.SensorConfigurationRequest;
import com.fruithapnotifier.app.service.requestadapter.requests.SensorRequest;
import org.greenrobot.eventbus.EventBus;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class RestRequestAdapter implements RequestAdapter
{
    private static final String LOGTAG = RestRequestAdapter.class.getName() ;
    private final ResultReceiver sensorRequestReceiver;
    private final ResultReceiver configurationRequestReceiver;
    private final Uri baseUri;
    private Context context;


    public RestRequestAdapter(final Context context)
    {
        this.context = context;
        SharedPreferences preferences = PreferenceManager.getDefaultSharedPreferences(this.context);
        String controlHost = preferences.getString("pref_server_hostname","localhost");
        int controlPort = Integer.valueOf(preferences.getString("pref_server_control_port","8888"));
        Uri.Builder uriBuilder = new Uri.Builder();
        uriBuilder.scheme("http").encodedAuthority(controlHost+":"+controlPort);
        baseUri = uriBuilder.build();

        sensorRequestReceiver = new ResultReceiver(new Handler()) {
            @Override
            protected void onReceiveResult(int resultCode, Bundle resultData)
            {

                String response = resultData.getString(Constants.REST_RESULT);

                if (resultCode == 200)
                {
                    try
                    {
                        JSONObject eventData = new JSONObject(response);
                        SensorEvent sensorEvent = new SensorEvent(eventData);
                        EventBus.getDefault().post(sensorEvent);

                    }
                    catch (JSONException e)
                    {
                        Log.e(LOGTAG, "Message error", e);
                        EventBus.getDefault().post(new RequestErrorEvent(e.getMessage()));

                    }
                }
                else
                {
                    String message = "StatefulSensor request failed with code: "+resultCode + " response= "+response;
                    Log.e(LOGTAG,message);
                    EventBus.getDefault().post(new RequestErrorEvent(message));
                }

            }
        };

        configurationRequestReceiver = new ResultReceiver(new Handler()) {
            @Override
            protected void onReceiveResult(int resultCode, Bundle resultData)
            {
                String response = resultData.getString(Constants.REST_RESULT);

                if (resultCode == 200)
                {
                    try
                    {
                        JSONArray configurationData = new JSONArray(response);
                        ConfigurationEvent configurationEvent = new ConfigurationEvent(configurationData);
                        EventBus.getDefault().post(configurationEvent);

                    }
                    catch (JSONException e)
                    {
                        Log.e(LOGTAG, "Message error", e);
                        EventBus.getDefault().post(new RequestErrorEvent(e.getMessage()));
                    }
                }
                else
                {
                    String msg = "Configuration request failed with code: " + resultCode + " Response=: " + response;
                    Log.e(LOGTAG, msg);
                    EventBus.getDefault().post(new RequestErrorEvent(msg));

                }
            }
        };



    }

    @Override
    public void sendSensorRequest(String sensorName, String operationName)
    {
        SensorRequest request = new SensorRequest(sensorName, operationName);
        RestConsumer.executeRequest(context,RestConsumer.GET,request.getUri(baseUri),null,sensorRequestReceiver);

    }

    @Override
    public void sendSensorConfigurationRequest(String sensorName)
    {
        SensorConfigurationRequest request = new SensorConfigurationRequest(sensorName);
        RestConsumer.executeRequest(context,RestConsumer.GET,request.getUri(baseUri),null, configurationRequestReceiver);
    }
}
