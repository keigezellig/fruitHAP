
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

package com.fruithapnotifier.app.ui.alerts;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Typeface;
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
        alertListItemViewHolder.getTxtMessage().setText(listItem.getNotificationText());
        alertListItemViewHolder.setItem(listItem);

        if (!listItem.isRead())
        {
            alertListItemViewHolder.getTxtTimestamp().setTypeface(alertListItemViewHolder.getTxtTimestamp().getTypeface(), Typeface.BOLD);
            alertListItemViewHolder.getTxtMessage().setTypeface(alertListItemViewHolder.getTxtMessage().getTypeface(), Typeface.BOLD);
        }
        else
        {
            alertListItemViewHolder.getTxtTimestamp().setTypeface(alertListItemViewHolder.getTxtTimestamp().getTypeface(), Typeface.NORMAL);
            alertListItemViewHolder.getTxtMessage().setTypeface(alertListItemViewHolder.getTxtMessage().getTypeface(), Typeface.NORMAL);

        }
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
        ImageView imgOptional = alertListItemDetailViewHolder.getImgOptional();
        if ( imageBytes != null)
        {
            Bitmap decodedImage = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
            imgOptional.setImageBitmap(decodedImage);
        }
        else
        {
            imgOptional.setImageBitmap(null);
        }
    }

    public AlertListItemViewModel findAlertByIdInAdapter(int id)
    {
        List<AlertListItemViewModel> itemList = (List<AlertListItemViewModel>)this.getParentItemList();
        for (AlertListItemViewModel item: itemList)
        {
            if (item.getId() == id)
            {
                return item;
            }
        }

        return null;
    }



}
