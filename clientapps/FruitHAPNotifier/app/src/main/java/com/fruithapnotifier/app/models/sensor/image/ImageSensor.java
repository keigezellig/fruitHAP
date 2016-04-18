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

package com.fruithapnotifier.app.models.sensor.image;

import android.content.Context;
import android.util.Base64;
import android.util.Log;
import com.fruithapnotifier.app.common.SensorEvent;
import com.fruithapnotifier.app.models.sensor.StatefulSensor;
import org.greenrobot.eventbus.EventBus;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;

/**
 * Created by MJOX03 on 18.4.2016.
 */
public class ImageSensor extends StatefulSensor
{
    private static final String TAG = "ImageSensor" ;

    public ImageSensor(String name, String description, String category, boolean isReadOnly, Context ctx)
    {
        super(name, description, category, isReadOnly, ctx);
    }

    @Override
    protected void handleSensorUpdateResponse(SensorEvent sensorEvent) throws JSONException
    {
        try
        {
            Log.d(TAG, "onSensorResponseReceived: This one is for image Sensor " + this.name);
            DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
            DateTime timestamp = new DateTime(fmt.parseDateTime(sensorEvent.getEventData().getString("TimeStamp")));
            String imageString = sensorEvent.getEventData().getJSONObject("Data").getJSONObject("Content").getString("Value");
            byte[] imageData = Base64.decode(imageString, Base64.DEFAULT);
            updateValue(imageData, timestamp);
        }
        catch (JSONException jex)
        {
            Log.e(TAG, "onSensorResponseReceived: Error while receiving update for sensor "+this.name, jex);
            updateValue(null, DateTime.now());
        }

    }

    private void updateValue(byte[] newValue, DateTime timestamp)
    {
        lastUpdated = timestamp;
        onValueChanged(new ImageValueChangeEvent(this,newValue, lastUpdated));
    }

    private void onValueChanged(ImageValueChangeEvent imageValueChangeEvent)
    {
        EventBus.getDefault().post(imageValueChangeEvent);
    }
}
