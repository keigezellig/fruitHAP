package com.fruithapnotifier.app.domain;

import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;
import org.json.JSONObject;
import org.joda.time.DateTime;



/**
 * Created by maarten on 12/2/15.
 */
public class SensorEvent
{
    private int id;
    private JSONObject eventData;

    public SensorEvent(int id, JSONObject eventData) {
        this.id = id;
        this.eventData = eventData;
    }

    public int getId()
    {
        return id;
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

    public Priority getNotificationPriority() throws JSONException
    {
        return Priority.values()[eventData.getJSONObject("Data").getInt("Priority")];
    }

    public JSONObject getOptionalData() throws JSONException
    {
        return eventData.getJSONObject("Data").getJSONObject("OptionalData");
    }

    @Override
    public String toString()
    {
        return "SensorEvent{" +
                "id=" + id +
                ", eventData=" + eventData +
                '}';
    }
}
