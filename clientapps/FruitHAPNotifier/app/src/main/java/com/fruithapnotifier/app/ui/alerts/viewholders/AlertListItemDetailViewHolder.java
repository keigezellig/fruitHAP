package com.fruithapnotifier.app.ui.alerts.viewholders;

import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import com.bignerdranch.expandablerecyclerview.ViewHolder.ChildViewHolder;
import com.fruithapnotifier.app.R;

/**
 * Created by developer on 2/2/16.
 */
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
