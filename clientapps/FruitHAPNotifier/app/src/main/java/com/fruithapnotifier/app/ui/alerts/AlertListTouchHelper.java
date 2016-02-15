package com.fruithapnotifier.app.ui.alerts;

import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.util.Log;
import com.bignerdranch.expandablerecyclerview.Adapter.ExpandableRecyclerAdapter;
import com.bignerdranch.expandablerecyclerview.ViewHolder.ParentViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemDetailViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewholders.AlertListItemViewHolder;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;

/**
 * Created by developer on 1/26/16.
 */
public class AlertListTouchHelper extends ItemTouchHelper.SimpleCallback {
    private AlertExpandableRecycleAdapter alertRecycleAdapter;

    public AlertListTouchHelper(AlertExpandableRecycleAdapter alertRecycleAdapter){
        super(ItemTouchHelper.UP | ItemTouchHelper.DOWN, ItemTouchHelper.LEFT | ItemTouchHelper.RIGHT);
        this.alertRecycleAdapter = alertRecycleAdapter;
    }

    @Override
    public boolean onMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
        //TODO: Not implemented here
        return false;
    }

    @Override
    public void onSwiped(RecyclerView.ViewHolder viewHolder, int direction) {
        AlertListItemViewModel parent = (AlertListItemViewModel)alertRecycleAdapter.getParentItemList().get(viewHolder.getAdapterPosition());
        alertRecycleAdapter.collapseParent(parent);
        int idx = alertRecycleAdapter.getParentItemList().indexOf(parent);
        Log.d(getClass().getName(),"Swiped: "+idx);
        alertRecycleAdapter.notifyParentItemRemoved(idx);


    }

    @Override
    public int getSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
    {
        if (viewHolder instanceof AlertListItemDetailViewHolder) return 0;
        return super.getSwipeDirs(recyclerView, viewHolder);
    }
}
