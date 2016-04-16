package com.fruithapnotifier.app.models.sensor;

import android.content.Context;
import android.util.Log;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.service.requestadapter.RestRequestAdapter;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;


public class Switch
{
    private static final String TAG = "Switch" ;
    private final RequestAdapter requestAdapter;
    private final boolean isReadÓnly;
    private String name;
    private String description;
    private String category;
    private SwitchState value;
    private DateTime lastUpdated;

    public String getName()
    {
        return name;
    }

    public String getDescription()
    {
        return description;
    }

    public String getCategory()
    {
        return category;
    }

    public boolean isReadÓnly()
    {
        return isReadÓnly;
    }

    public void updateValue(SwitchState newValue, DateTime timestamp)
    {
        if (newValue != value)
        {
            Log.d(TAG,"Value changed: "+this);
            value = newValue;
            lastUpdated = timestamp;
            onValueChanged(new SwitchChangeEvent(this,value, lastUpdated));
        }
    }

    private void onValueChanged(SwitchChangeEvent switchChangedEvent)
    {
        EventBus.getDefault().post(switchChangedEvent);
    }

    public Switch(String name, String description, String category, boolean isReadOnly, Context ctx)
    {
        this.name = name;
        this.description = description;
        this.category = category;
        this.isReadÓnly = isReadOnly;
        requestAdapter = new RestRequestAdapter(ctx);
        EventBus.getDefault().register(this);
    }

    public void requestUpdate()
    {
        requestAdapter.sendSensorRequest(this.name, "GetValue");
    }

    public void turnOn()
    {
        if (!isReadÓnly)
        {
            requestAdapter.sendSensorRequest(this.name, "TurnOn");
        }
        else
        {
            Log.w(TAG, "turnOn: Cannot change state of a read only switch");
        }
    }

    public void turnOff()
    {
        if (!isReadÓnly)
        {
            requestAdapter.sendSensorRequest(this.name, "TurnOff");
        }
        else
        {
            Log.w(TAG, "turnOff: Cannot change state of a read only switch");
        }
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
                try
                {
                    Log.d(TAG, "onSensorResponseReceived: This one is for switch " + this.name);
                    DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
                    DateTime timestamp = new DateTime(fmt.parseDateTime(sensorEvent.getEventData().getString("TimeStamp")));
                    int state = sensorEvent.getEventData().getJSONObject("Data").getJSONObject("Content").getInt("Value");
                    SwitchState value = SwitchState.values()[state];
                    updateValue(value, timestamp);
                }
                catch (JSONException jex)
                {
                    Log.e(TAG, "onSensorResponseReceived: Error while receiving update for sensor "+this.name, jex);
                    updateValue(SwitchState.UNDEFINED, DateTime.now());
                }

            }
        }
        catch (JSONException jex)
        {
            Log.e(TAG, "onSensorResponseReceived: Invalid data received", jex);
        }



    }

    public void subscribeToUpdates()
    {
        EventBus.getDefault().register(this);
    }

    public void unsubscribeToUpdates()
    {
        EventBus.getDefault().unregister(this);
    }

    @Override
    public String toString()
    {
        return "Switch{" +
                "name='" + name + '\'' +
                ", description='" + description + '\'' +
                ", category='" + category + '\'' +
                ", value=" + value +
                ", lastUpdated=" + lastUpdated +
                '}';
    }
}

