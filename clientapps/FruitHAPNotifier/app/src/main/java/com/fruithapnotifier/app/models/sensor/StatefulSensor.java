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

package com.fruithapnotifier.app.models.sensor;

import android.content.Context;
import android.util.Log;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.service.requestadapter.RestRequestAdapter;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.joda.time.DateTime;
import org.json.JSONException;

public abstract class StatefulSensor implements Sensor
{
    protected static final String TAG = StatefulSensor.class.getName();

    protected final RequestAdapter requestAdapter;
    protected final boolean isReadOnly;
    protected String name;
    protected String description;
    protected String category;
    protected DateTime lastUpdated;

    public StatefulSensor(String name, String description, String category, boolean isReadOnly, Context ctx)
    {
        this.description = description;
        this.category = category;

        this.isReadOnly = isReadOnly;
        this.name = name;
        requestAdapter = new RestRequestAdapter(ctx);
    }

    @Override
    public String getName()
    {
        return name;
    }

    @Override
    public String getDescription()
    {
        return description;
    }

    @Override
    public String getCategory()
    {
        return category;
    }

    @Override
    public boolean isReadOnly()
    {
        return isReadOnly;
    }

    public void requestUpdate()
    {
        requestAdapter.sendSensorRequest(this.name, "GetValue");
    }

    @Subscribe
    public void onSensorResponseReceived(SensorEvent sensorEvent)
    {
        try
        {
            String sensorName = sensorEvent.getEventData().getString("SensorName");
            String typeName = sensorEvent.getEventData().getJSONObject("Data").getString("TypeName");

            Log.d(TAG, "onSensorResponseReceived: Received response:" + sensorEvent.getEventData());

            if (sensorName.equals(this.name) && typeName.equals("OnOffValue"))
            {
                handleSensorUpdateResponse(sensorEvent);
            }
        }
        catch (JSONException jex)
        {
            Log.e(TAG, "onSensorResponseReceived: Invalid data received", jex);
        }
    }

    protected abstract void handleSensorUpdateResponse(SensorEvent sensorEvent) throws JSONException;

    @Override
    public void registerForEvents()
    {
        EventBus.getDefault().register(this);
    }

    @Override
    public void unregisterForEvents()
    {
        EventBus.getDefault().unregister(this);
    }


}
