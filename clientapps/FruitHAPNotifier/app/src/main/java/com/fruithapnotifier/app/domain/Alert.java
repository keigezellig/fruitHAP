package com.fruithapnotifier.app.domain;

import android.util.Log;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;
import org.json.JSONObject;
import org.joda.time.DateTime;



/**
 * Created by maarten on 12/2/15.
 */
public class Alert extends Event
{

    public Alert(int id, JSONObject eventData) {
        super(eventData, id);
    }

    public String getSensorName() throws JSONException
    {
        return eventData.getString("SensorName");
    }

    public DateTime getTimestamp() throws JSONException
    {
        DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
        DateTime timestamp = new DateTime(fmt.parseDateTime(eventData.getString("TimeStamp")));
        return timestamp;
    }

    public String getNotificationText() throws JSONException
    {
        return eventData.getJSONObject("Data").getString("NotificationText");
    }

    public AlertPriority getNotificationPriority() throws JSONException
    {
        return AlertPriority.values()[eventData.getJSONObject("Data").getInt("AlertPriority")];
    }

    public JSONObject getOptionalData() throws JSONException
    {
        return eventData.getJSONObject("Data").getJSONObject("OptionalData");
    }

    @Override
    public String toString()
    {
        try
        {
            String dt = DateTimeFormat.forStyle("SS").print(getTimestamp());
            return String.format("%s %s %s",getNotificationPriority(),dt,getNotificationText());
        }
        catch (JSONException e)
        {
            Log.e(this.getClass().getName(),"Error",e);
            return "";
        }

    }
}
