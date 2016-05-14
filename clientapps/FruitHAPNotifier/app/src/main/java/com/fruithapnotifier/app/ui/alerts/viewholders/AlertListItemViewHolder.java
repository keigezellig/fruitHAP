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
import android.widget.TextView;
import com.bignerdranch.expandablerecyclerview.ViewHolder.ParentViewHolder;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;


public class AlertListItemViewHolder extends ParentViewHolder
{

    private TextView txtPriority;
    private TextView txtTimestamp;
    private TextView txtMessage;
    private AlertListItemViewModel item;

    public AlertListItemViewHolder(View itemView)
    {
        super(itemView);
        txtPriority = (TextView) itemView.findViewById(R.id.alert_listitem_txtPriority);
        txtTimestamp = (TextView) itemView.findViewById(R.id.alert_listitem_txtTimestamp);
        txtMessage = (TextView) itemView.findViewById(R.id.alert_listitem_txtMessage);

    }

    public TextView getTxtPriority()
    {
        return txtPriority;
    }

    public TextView getTxtTimestamp()
    {
        return txtTimestamp;
    }

    public TextView getTxtMessage()
    {
        return txtMessage;
    }

    public void setItem(AlertListItemViewModel item)
    {
        this.item = item;
    }

    public AlertListItemViewModel getItem()
    {
        return item;
    }
}
