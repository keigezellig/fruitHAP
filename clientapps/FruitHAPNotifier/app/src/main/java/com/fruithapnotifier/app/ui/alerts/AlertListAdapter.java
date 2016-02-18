package com.fruithapnotifier.app.ui.alerts;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.GradientDrawable;
import android.support.annotation.NonNull;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import com.bignerdranch.expandablerecyclerview.Adapter.ExpandableRecyclerAdapter;
import com.bignerdranch.expandablerecyclerview.Model.ParentListItem;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemDetailViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemDetailViewModel;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;

import java.util.List;

/**
 * Created by developer on 1/26/16.
 */
public class AlertListAdapter extends ExpandableRecyclerAdapter<AlertListItemViewHolder, AlertListItemDetailViewHolder>
{

    private final LayoutInflater inflater;
    private Context context;

    public AlertListAdapter(Context context, @NonNull List<? extends ParentListItem> parentItemList)
    {
        super(parentItemList);
        this.context = context;
        inflater = LayoutInflater.from(context);
    }

    @Override
    public AlertListItemViewHolder onCreateParentViewHolder(ViewGroup viewGroup)
    {
        View view = inflater.inflate(R.layout.alert_listitem, viewGroup, false);
        return new AlertListItemViewHolder(view);
    }

    @Override
    public AlertListItemDetailViewHolder onCreateChildViewHolder(ViewGroup viewGroup)
    {
        View view = inflater.inflate(R.layout.alert_fragment_detail, viewGroup, false);
        return new AlertListItemDetailViewHolder(view);
    }

    @Override
    public void onBindParentViewHolder(AlertListItemViewHolder alertListItemViewHolder, int i, ParentListItem parentListItem)
    {
        AlertListItemViewModel listItem = (AlertListItemViewModel)parentListItem;
        GradientDrawable background = (GradientDrawable) alertListItemViewHolder.getTxtPriority().getBackground();
        background.setColor(listItem.getPriorityColor());
        alertListItemViewHolder.getTxtPriority().setText(listItem.getPriorityText());
        alertListItemViewHolder.getTxtTimestamp().setText(listItem.getTimestamp());
        alertListItemViewHolder.getTxtMessage().setText(listItem.getId() + "." + listItem.getNotificationText());
    }

    @Override
    public void onBindChildViewHolder(AlertListItemDetailViewHolder alertListItemDetailViewHolder, int i, Object o)
    {
        AlertListItemDetailViewModel detailItem = (AlertListItemDetailViewModel)o;

        GradientDrawable background = (GradientDrawable)alertListItemDetailViewHolder.getTxtPriority().getBackground();
        background.setColor(detailItem.getPriorityColor());
        alertListItemDetailViewHolder.getTxtPriority().setText(detailItem.getPriorityText());
        alertListItemDetailViewHolder.getTxtTimestamp().setText(detailItem.getTimestamp());
        alertListItemDetailViewHolder.getTxtSensorName().setText(detailItem.getSensorName());
        alertListItemDetailViewHolder.getTxtMessage().setText(detailItem.getNotificationText());

        byte[] imageBytes = detailItem.getImage();
        if ( imageBytes != null)
        {
            Bitmap decodedImage = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
            ImageView imgOptional = alertListItemDetailViewHolder.getImgOptional();
            imgOptional.setImageBitmap(decodedImage);
        }
    }

    public AlertListItemViewModel findAlertByIdInAdapter(int id)
    {
        for (AlertListItemViewModel item: (List<AlertListItemViewModel>)this.getParentItemList())
        {
            if (item.getId() == id)
            {
                return item;
            }
        }

        return null;
    }



}
