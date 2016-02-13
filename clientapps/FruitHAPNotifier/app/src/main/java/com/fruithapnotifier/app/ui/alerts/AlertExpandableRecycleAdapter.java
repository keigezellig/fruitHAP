package com.fruithapnotifier.app.ui.alerts;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.GradientDrawable;
import android.support.annotation.NonNull;
import android.support.v7.widget.RecyclerView;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import com.bignerdranch.expandablerecyclerview.Adapter.ExpandableRecyclerAdapter;
import com.bignerdranch.expandablerecyclerview.Model.ParentListItem;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.EventRepository;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemDetailViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemDetailViewModel;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;

import java.util.List;

/**
 * Created by developer on 1/26/16.
 */
public class AlertExpandableRecycleAdapter extends ExpandableRecyclerAdapter<AlertListItemViewHolder, AlertListItemDetailViewHolder>
{

    private final LayoutInflater inflater;
    private Context context;

    public AlertExpandableRecycleAdapter(Context context, @NonNull List<? extends ParentListItem> parentItemList)
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
        alertListItemViewHolder.getTxtMessage().setText(listItem.getNotificationText());

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



}
