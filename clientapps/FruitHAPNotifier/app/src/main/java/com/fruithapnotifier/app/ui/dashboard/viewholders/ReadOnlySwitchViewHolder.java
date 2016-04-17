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

package com.fruithapnotifier.app.ui.dashboard.viewholders;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import com.fruithapnotifier.app.R;

public class ReadOnlySwitchViewHolder extends RecyclerView.ViewHolder
{

    private final TextView txtLastupdated;
    private final TextView txtName;
    private final TextView txtDesc;
    private final ImageView swState;

    public ReadOnlySwitchViewHolder(View itemView)
    {
        super(itemView);
        txtName = (TextView) itemView.findViewById(R.id.dashboard_roswitch_txtName);
        txtDesc = (TextView) itemView.findViewById(R.id.dashboard_roswitch_txtDescription);
        txtLastupdated = (TextView) itemView.findViewById(R.id.dashboard_roswitch_txtLastUpdated);
        swState = (ImageView)itemView.findViewById(R.id.dashboard_roswitch_swState);
    }

    public TextView getTxtLastupdated()
    {
        return txtLastupdated;
    }

    public TextView getTxtName()
    {
        return txtName;
    }

    public TextView getTxtDesc()
    {
        return txtDesc;
    }

    public ImageView getSwState()
    {
        return swState;
    }

}
