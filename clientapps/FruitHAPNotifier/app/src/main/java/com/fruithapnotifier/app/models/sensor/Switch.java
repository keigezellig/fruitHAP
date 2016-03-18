package com.fruithapnotifier.app.models.sensor;

import android.content.Context;
import android.util.Log;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.service.requestadapter.MessageQueueRequestAdapter;
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

    public Switch(String name, String description, String category, Context ctx )
    {
        this.name = name;
        this.description = description;
        this.category = category;
        requestAdapter = new MessageQueueRequestAdapter(ctx);
        EventBus.getDefault().register(this);
    }

    public void requestUpdate()
    {
        requestAdapter.sendSensorRequest(this.name, "GetValue", null);
    }

    public void turnOn()
    {
        requestAdapter.sendSensorRequest(this.name, "TurnOn", null);
    }

    public void turnOff()
    {
        requestAdapter.sendSensorRequest(this.name, "TurnOff", null);
    }

    @Subscribe
    public void onSensorResponseReceived(SensorEvent sensorEvent)
    {

        DateTime timestamp = new DateTime();
        SwitchState value = SwitchState.UNDEFINED;
        String eventType =  sensorEvent.getEventData().optString("EventType", "");
        String sensorName = sensorEvent.getEventData().optString("SensorName", "");

        Log.d(TAG, "onSensorResponseReceived: Received response:"+sensorEvent.getEventData());

        if (sensorName.equals(this.name) && ( eventType.equals("SensorEvent") || eventType.equals("GetValue") ))
        {
            try
            {
                Log.d(TAG, "onSensorResponseReceived: This one is for " + this.name);
                DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
                timestamp = new DateTime(fmt.parseDateTime(sensorEvent.getEventData().getString("TimeStamp")));
                int state = sensorEvent.getEventData().getJSONObject("Data").getInt("Content");
                value = SwitchState.values()[state];
            }
            catch (JSONException jex)
            {
                Log.e(TAG, "onSensorResponseReceived: Error while receiving update", jex);
            }

            updateValue(value,timestamp);
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

