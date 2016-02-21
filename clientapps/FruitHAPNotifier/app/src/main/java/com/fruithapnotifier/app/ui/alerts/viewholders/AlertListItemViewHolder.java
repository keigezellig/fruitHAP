package com.fruithapnotifier.app.ui.alerts.viewholders;

import android.util.Log;
import android.view.View;
import android.widget.TextView;
import com.bignerdranch.expandablerecyclerview.Model.ParentListItem;
import com.bignerdranch.expandablerecyclerview.ViewHolder.ParentViewHolder;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;

/**
 * Created by developer on 2/2/16.
 */
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
