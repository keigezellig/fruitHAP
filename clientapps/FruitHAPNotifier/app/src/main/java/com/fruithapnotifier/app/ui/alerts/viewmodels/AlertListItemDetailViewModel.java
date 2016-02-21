package com.fruithapnotifier.app.ui.alerts.viewmodels;

import android.graphics.Color;

/**
 * Created by developer on 2/2/16.
 */
public class AlertListItemDetailViewModel
{
    private final String timestamp;
    private final String sensorName;
    private final String notificationText;
    private final String priorityText;
    private final int priorityColor;
    private final byte[] image;

    public AlertListItemDetailViewModel(String timestamp, String sensorName, String notificationText, String priorityText, int priorityColor, byte[] image)
    {

        this.timestamp = timestamp;
        this.sensorName = sensorName;
        this.notificationText = notificationText;
        this.priorityText = priorityText;
        this.priorityColor = priorityColor;
        this.image = image;
    }

    public String getTimestamp()
    {
        return timestamp;
    }

    public String getSensorName()
    {
        return sensorName;
    }

    public String getNotificationText()
    {
        return notificationText;
    }

    public String getPriorityText()
    {
        return priorityText;
    }

    public int getPriorityColor()
    {
        return priorityColor;
    }

    public byte[] getImage()
    {
        return image;
    }
}
