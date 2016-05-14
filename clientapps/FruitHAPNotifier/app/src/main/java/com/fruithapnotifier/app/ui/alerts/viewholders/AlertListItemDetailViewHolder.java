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

package com.fruithapnotifier.app.ui.alerts.viewholders;

import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import com.bignerdranch.expandablerecyclerview.ViewHolder.ChildViewHolder;
import com.fruithapnotifier.app.R;


public class AlertListItemDetailViewHolder extends ChildViewHolder
{

    private final TextView txtPriority;
    private final TextView txtTimestamp;
    private final TextView txtSensorName;
    private final TextView txtMessage;
    private final ImageView imgOptional;

    public AlertListItemDetailViewHolder(View itemView)
    {
        super(itemView);
        txtPriority = (TextView) itemView.findViewById(R.id.alert_detail_txtPriority);
        txtTimestamp = (TextView) itemView.findViewById(R.id.alert_detail_txtTimestamp);
        txtSensorName = (TextView) itemView.findViewById(R.id.alert_detail_txtSensorName);
        txtMessage = (TextView) itemView.findViewById(R.id.alert_detail_txtMessage);
        imgOptional = (ImageView) itemView.findViewById(R.id.alert_detail_imgOptional);
    }

    public TextView getTxtPriority()
    {
        return txtPriority;
    }

    public TextView getTxtTimestamp()
    {
        return txtTimestamp;
    }

    public TextView getTxtSensorName()
    {
        return txtSensorName;
    }

    public TextView getTxtMessage()
    {
        return txtMessage;
    }

    public ImageView getImgOptional()
    {
        return imgOptional;
    }
}
