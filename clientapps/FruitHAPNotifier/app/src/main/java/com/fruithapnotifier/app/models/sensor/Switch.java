package com.fruithapnotifier.app.models.sensor;

import android.content.Context;
import android.util.Log;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchViewStateChangeEvent;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;


public class Switch extends StatefulSensor
{
    private static final String TAG = "Switch" ;
    private SwitchState value;

    private void updateValue(SwitchState newValue, DateTime timestamp)
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
        super(name, description, category, isReadOnly, ctx);

    }

    private void turnOn()
    {
        if (!isReadOnly)
        {
            requestAdapter.sendSensorRequest(this.name, "TurnOn");
        }
        else
        {
            Log.w(TAG, "turnOn: Cannot change state of a read only switch");
        }
    }

    private void turnOff()
    {
        if (!isReadOnly)
        {
            requestAdapter.sendSensorRequest(this.name, "TurnOff");
        }
        else
        {
            Log.w(TAG, "turnOff: Cannot change state of a read only switch");
        }
    }

    @Subscribe
    public void onSwitchViewStateChangeReceived(SwitchViewStateChangeEvent viewStateChangeEvent)
    {
        if (viewStateChangeEvent.getName().equals(this.name))
        {
            if (viewStateChangeEvent.isOn())
            {
                turnOn();
            }
            else
            {
                turnOff();
            }
        }
    }

    @Override
    protected void handleSensorUpdateResponse(SensorEvent sensorEvent) throws JSONException
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

