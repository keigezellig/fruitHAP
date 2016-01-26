package com.fruithapnotifier.app.ui.alerts;

import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;

/**
 * Created by developer on 1/26/16.
 */
public class AlertListTouchHelper extends ItemTouchHelper.SimpleCallback {
    private AlertRecycleAdapter alertRecycleAdapter;

    public AlertListTouchHelper(AlertRecycleAdapter alertRecycleAdapter){
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
        //Remove item
        alertRecycleAdapter.deleteFromList(viewHolder.getAdapterPosition());
    }
}
