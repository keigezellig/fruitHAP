package com.fruithapnotifier.app.domain;

import android.os.Parcel;
import android.os.Parcelable;
import org.json.JSONObject;

/**
 * Created by developer on 1/18/16.
 */
public class Event implements Parcelable
{
    protected int id;
    protected JSONObject eventData;

    public Event(JSONObject eventData, int id)
    {
        this.eventData = eventData;
        this.id = id;
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

    protected Event(Parcel in)
    {
        this.id = in.readInt();
        this.eventData = in.readParcelable(JSONObject.class.getClassLoader());
    }

    public static final Parcelable.Creator<Event> CREATOR = new Parcelable.Creator<Event>()
    {
        public Event createFromParcel(Parcel source)
        {
            return new Event(source);
        }

        public Event[] newArray(int size)
        {
            return new Event[size];
        }
    };
}
