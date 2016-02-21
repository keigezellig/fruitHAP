package com.fruithapnotifier.app.ui.helpers;

import android.graphics.Color;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.AlertPriority;

/**
 * Created by developer on 1/2/16.
 */
public class PriorityHelpers
{
    public static int convertToColor(AlertPriority priority)
    {
        switch (priority)
        {
            case Low:
                return Color.WHITE;
            case Medium:
                return Color.YELLOW;
            case High:
                return Color.RED;
            default:
                return Color.BLACK;
        }
    }

    public static int getTextResource(AlertPriority priority)
    {
        switch (priority)
        {
            case Low:
                return R.string.alert_details_priority_low;
            case Medium:
                return R.string.alert_details_priority_medium;
            case High:
                return R.string.alert_details_priority_high;
            default:
                return -1;
        }
    }

    public static String getText(AlertPriority priority)
    {
        switch (priority)
        {
            case Low:
                return "Low";
            case Medium:
                return "Medium";
            case High:
                return "High";
            default:
                return "";
        }
    }


}