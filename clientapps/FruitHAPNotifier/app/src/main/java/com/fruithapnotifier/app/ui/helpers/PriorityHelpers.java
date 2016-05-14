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

package com.fruithapnotifier.app.ui.helpers;

import android.graphics.Color;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.models.alert.AlertPriority;


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
