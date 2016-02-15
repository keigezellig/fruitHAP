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

    private final int id;
    private final String timestamp;
    private final String notificationText;
    private final String priorityText;
    private final int priorityColor;
    private List<AlertListItemDetailViewModel> childList;

    public AlertListItemViewModel(int id, String timestamp, String notificationText, String priorityText, int priorityColor)
    {
        this.id = id;
        this.timestamp = timestamp;
        this.notificationText = notificationText;
        this.priorityText = priorityText;
        this.priorityColor = priorityColor;
    }


    public int getId()
    {
        return this.id;
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

    public List<AlertListItemDetailViewModel> getChildItemList()
    {
        return childList;
    }

    public void setChildItemList(List<AlertListItemDetailViewModel> list)
    {
        childList = list;
    }


    @Override
    public boolean isInitiallyExpanded()
    {
        return false;
    }

    @Override
    public boolean equals(Object o)
    {
        if (this == o)
        {
            return true;
        }
        if (o == null || getClass() != o.getClass())
        {
            return false;
        }

        AlertListItemViewModel that = (AlertListItemViewModel) o;

        return id == that.id;

    }

    @Override
    public int hashCode()
    {
        return id;
    }
}
