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

package com.fruithapnotifier.app.models.sensor.text;

import android.content.Context;
import android.util.Log;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.models.sensor.StatefulSensor;
import org.greenrobot.eventbus.EventBus;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;
import org.json.JSONObject;

/**
 * Created by MJOX03 on 18.4.2016.
 */
public class TextSensor extends StatefulSensor
{
    private static final String TAG = "TextSensor" ;
    private String value;

    public TextSensor(String name, String description, String category, boolean isReadOnly, Context ctx)
    {
        super(name, description, category, isReadOnly, ctx);
    }

    @Override
    protected void handleSensorUpdateResponse(JSONObject eventData) throws JSONException
    {
        try
        {
            String sensorName = eventData.getString("SensorName");
            String typeName = eventData.getJSONObject("Data").getString("TypeName");

            if (sensorName.equals(this.name) && typeName.equals("TextValue"))
            {
                Log.d(TAG, "onSensorResponseReceived: This one is for text Sensor " + this.name);
                DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
                DateTime timestamp = new DateTime(fmt.parseDateTime(eventData.getString("TimeStamp")));
                String value = eventData.getJSONObject("Data").getJSONObject("Content").getString("Value");
                updateValue(value, timestamp);
            }
        }
        catch (JSONException jex)
        {
            Log.e(TAG, "onSensorResponseReceived: Error while receiving update for sensor "+this.name, jex);
            updateValue(null, DateTime.now());
        }

    }

    private void updateValue(String newValue, DateTime timestamp)
    {
        if (newValue != value)
        {
            Log.d(TAG, "Value changed: " + this);
            lastUpdated = timestamp;
            onValueChanged(new TextValueChangeEvent(this, newValue, lastUpdated));
        }
    }

    private void onValueChanged(TextValueChangeEvent textValueChangeEvent)
    {
        EventBus.getDefault().post(textValueChangeEvent);
    }
}
