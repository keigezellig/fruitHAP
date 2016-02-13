package com.fruithapnotifier.app.ui.alerts.viewmodels;

import com.bignerdranch.expandablerecyclerview.Model.ParentListItem;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by developer on 2/2/16.
 */
public class AlertListItemViewModel implements ParentListItem
{
    public AlertListItemDetailViewModel childModel;

    private final String timestamp;
    private final String notificationText;
    private final String priorityText;
    private final int priorityColor;
    private ArrayList<AlertListItemDetailViewModel> childList;

    public AlertListItemViewModel(String timestamp, String sensorName, String notificationText, String priorityText, int priorityColor, byte[] image)
    {
        this.timestamp = timestamp;
        this.notificationText = notificationText;
        this.priorityText = priorityText;
        this.priorityColor = priorityColor;

        childModel = new AlertListItemDetailViewModel(timestamp, sensorName, notificationText, priorityText, priorityColor, image);
        childList = new ArrayList<>();
    }


    public String getTimestamp()
    {
        return timestamp;
    }

    public String getPriorityText()
    {
        return priorityText;
    }

    public int getPriorityColor()
    {
        return priorityColor;
    }

    public String getNotificationText()
    {
        return notificationText;
    }

    @Override
    public List<?> getChildItemList()
    {

        childList.add(childModel);
        return childList;
    }

    @Override
    public boolean isInitiallyExpanded()
    {
        return false;
    }
}
