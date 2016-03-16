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
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.SensorUpdateEvent;
import com.fruithapnotifier.app.service.FruithapRpcService;
import com.fruithapnotifier.app.service.requestadapter.requests.SensorRequest;
import org.apache.commons.lang3.NotImplementedException;
import org.greenrobot.eventbus.EventBus;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Dictionary;

public class FruithapRpcServiceRequestAdapter implements RequestAdapter
{
    private static final String LOGTAG = FruithapRpcServiceRequestAdapter.class.getName() ;
    private final ResultReceiver receiver;
    private Context context;

    public FruithapRpcServiceRequestAdapter(Context context)
    {
        this.context = context;
        receiver = new ResultReceiver(new Handler()) {
            @Override
            protected void onReceiveResult(int resultCode, Bundle resultData)
            {
                if (resultCode == Constants.RPC_REQUEST_OK)
                {
                    String response = resultData.getString(Constants.RPC_RESULTDATA);
                    try
                    {
                        JSONObject eventData = new JSONObject(response);
                        SensorUpdateEvent updateEvent = new SensorUpdateEvent(eventData);
                        EventBus.getDefault().post(updateEvent);

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
    public void sendSensorRequest(String sensorName, String operationName, Dictionary<String, String> parameters)
    {
        SensorRequest request = new SensorRequest(sensorName,operationName,parameters);
        FruithapRpcService.executeSensorRequest(context,request.toJson().toString(),receiver);

    }

    @Override
    public void sendConfigurationRequest(String operationName, Dictionary<String, String> parameters)
    {
        throw new NotImplementedException("Not implemented yet");
    }
}
