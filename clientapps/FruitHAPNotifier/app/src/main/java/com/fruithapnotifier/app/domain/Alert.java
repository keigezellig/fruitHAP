package com.fruithapnotifier.app.domain;

import android.os.Parcel;
import android.os.Parcelable;
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
public class Alert implements Parcelable {

    public static final Creator<Alert> CREATOR = new Creator<Alert>()
    {
        public Alert createFromParcel(Parcel source)
        {
            return new Alert(source);
        }

        public Alert[] newArray(int size)
        {
            return new Alert[size];
        }
    };
    private int id;
    private JSONObject eventData;



    public Alert(int id, JSONObject eventData)
    {
        this.eventData = eventData;
        this.id = id;
    }


    protected Alert(Parcel in)
    {
        this.id = in.readInt();
        this.eventData = in.readParcelable(JSONObject.class.getClassLoader());
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

    public int getId()
    {
        return id;
    }

    @Override
    public int describeContents()
    {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel dest, int flags)
    {
        dest.writeInt(this.id);
        dest.writeString(this.eventData.toString());
    }
}
