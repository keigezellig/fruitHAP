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

   public static String TAG = Alert.class.getName();
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
    private boolean isRead;

    private int id;
    private String sensorName;
    private DateTime timestamp;
    private String notificationText;
    private AlertPriority notificationPriority;
    private JSONObject optionalData;


    public Alert(int id, DateTime timestamp, String sensorName, String notificationText, AlertPriority notificationPriority, JSONObject optionalData, boolean hasBeenRead)
    {
        this.id = id;
        this.sensorName = sensorName;
        this.timestamp = timestamp;
        this.notificationText = notificationText;
        this.notificationPriority = notificationPriority;
        this.optionalData = optionalData;
        this.isRead = hasBeenRead;
    }


    protected Alert(Parcel in)
    {
        this.id = in.readInt();
        this.timestamp = (DateTime) in.readValue(DateTime.class.getClassLoader());
        this.sensorName = in.readString();
        this.notificationText = in.readString();
        this.notificationPriority = AlertPriority.values()[in.readInt()];
        this.optionalData = (JSONObject) in.readValue(JSONObject.class.getClassLoader());
        this.isRead = in.readInt() != 0;
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
        dest.writeValue(this.timestamp);
        dest.writeString(this.sensorName);
        dest.writeString(this.notificationText);
        dest.writeInt(this.notificationPriority.ordinal());
        dest.writeValue(this.optionalData);
        dest.writeInt((int) (this.isRead ? 1 : 0));
    }


    public int getId() {
        return id;
    }

    public String getSensorName() {
        return sensorName;
    }

    public DateTime getTimestamp() {
        return timestamp;
    }

    public String getNotificationText() {
        return notificationText;
    }

    public AlertPriority getNotificationPriority() {
        return notificationPriority;
    }

    public JSONObject getOptionalData() {
        return optionalData;
    }

    public boolean isRead()
    {
        return isRead;
    }

    public void setRead(boolean read)
    {
        isRead = read;
    }

    @Override
    public String toString() {
        return "Alert{" +
                "id=" + id +
                ", sensorName='" + sensorName + '\'' +
                ", timestamp=" + timestamp +
                ", notificationText='" + notificationText + '\'' +
                ", notificationPriority=" + notificationPriority +
                ", optionalData=" + optionalData +
                '}';
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        Alert alert = (Alert) o;

        return id == alert.id;

    }

    @Override
    public int hashCode() {
        return id;
    }

    public static Alert createAlertFromEventData(JSONObject eventData)
    {
       Log.d(TAG,eventData.toString());
        try {
           int id = -1;
           String sensorName = eventData.getString("SensorName");
           DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
           DateTime timestamp = new DateTime(fmt.parseDateTime(eventData.getString("TimeStamp")));
           AlertPriority prio = AlertPriority.values()[eventData.getJSONObject("Data").getInt("Priority")];
           String text = eventData.getJSONObject("Data").getString("NotificationText");
           JSONObject optionalData = null;

           if (!eventData.getJSONObject("Data").isNull("OptionalData"))
           {
               optionalData = eventData.getJSONObject("Data").getJSONObject("OptionalData");
           }

           return new Alert(id, timestamp, sensorName, text, prio,optionalData, false);
       }
       catch (JSONException ex)
       {
           Log.e(TAG,"Cannot parse event data",ex);
       }

        return null;

    }
}
