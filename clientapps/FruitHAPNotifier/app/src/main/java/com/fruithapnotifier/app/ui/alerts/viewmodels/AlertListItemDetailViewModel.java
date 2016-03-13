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
