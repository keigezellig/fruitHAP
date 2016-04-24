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

package com.fruithapnotifier.app.ui.dashboard.viewholders.button;

import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import com.fruithapnotifier.app.R;

public class ButtonViewHolder extends RecyclerView.ViewHolder
{
    private final TextView txtName;
    private final TextView txtDesc;
    private final Button btnExecute;

    public ButtonViewHolder(View itemView)
    {
        super(itemView);
        txtName = (TextView) itemView.findViewById(R.id.dashboard_button_txtName);
        txtDesc = (TextView) itemView.findViewById(R.id.dashboard_button_txtDescription);
        btnExecute = (Button)itemView.findViewById(R.id.dashboard_button_btnExecute);
    }

    public TextView getTxtName()
    {
        return txtName;
    }

    public TextView getTxtDesc()
    {
        return txtDesc;
    }

    public Button getBtnExecute()
    {
        return btnExecute;
    }

}
