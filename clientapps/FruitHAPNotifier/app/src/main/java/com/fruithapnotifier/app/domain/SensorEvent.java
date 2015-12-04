package com.fruithapnotifier.app.domain;

import java.util.Date;

/**
 * Created by maarten on 12/2/15.
 */
public class SensorEvent
{
    private int id;
    private String sensorName;
    private Date timestamp;
    private byte[] optionalData;

    public SensorEvent(int id, String sensorName, Date timestamp, byte[] optionalData) {
        this.id = id;
        this.sensorName = sensorName;
        this.timestamp = timestamp;
        this.optionalData = optionalData;
    }

    public int getId() {
        return id;
    }

    public String getSensorName() {
        return sensorName;
    }

    public Date getTimestamp() {
        return timestamp;
    }

    public byte[] getOptionalData() {
        return optionalData;
    }

    @Override
    public String toString() {
        return getTimestamp().toString() + " " + getSensorName();
    }
}
