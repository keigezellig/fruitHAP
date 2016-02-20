package com.fruithapnotifier.app.ui.alerts;

import android.content.Context;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.util.Log;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemDetailViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;

/**
 * Created by developer on 1/26/16.
 */
public class AlertListTouchHelper extends ItemTouchHelper.SimpleCallback {
    private final Context ctx;
    private AlertListAdapter alertRecycleAdapter;

    public AlertListTouchHelper(AlertListAdapter alertRecycleAdapter, Context ctx){
        super(ItemTouchHelper.UP | ItemTouchHelper.DOWN, ItemTouchHelper.LEFT | ItemTouchHelper.RIGHT);
        this.alertRecycleAdapter = alertRecycleAdapter;
        this.ctx = ctx;
    }

    @Override
    public boolean onMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
        return false;
    }


    @Override
    public void onSwiped(RecyclerView.ViewHolder viewHolder, int direction)
    {
        AlertListItemViewHolder vh = (AlertListItemViewHolder) viewHolder;
        AlertListItemViewModel parent = vh.getItem();
        Log.d(getClass().getName(), "Deleting: " + parent.getId());
        AlertRepository datasource = new AlertRepository(ctx);
        datasource.deleteAlert(datasource.getAlertById(parent.getId()));
    }

    @Override
    public int getSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
    {
        if (viewHolder instanceof AlertListItemDetailViewHolder)
        {
            return 0;
        }

        AlertListItemViewHolder vh = (AlertListItemViewHolder)viewHolder;
        if (vh.isExpanded())
        {
            return 0;
        }

        return super.getSwipeDirs(recyclerView, viewHolder);
    }
}
