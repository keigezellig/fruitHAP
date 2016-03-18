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

package com.fruithapnotifier.app.ui.dashboard;

import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CompoundButton;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.ui.dashboard.viewholders.SwitchViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchState;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchViewModel;

import java.util.List;


public class DashboardAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder>
{
    private List<SwitchViewModel> items;
    private final int SWITCHITEM = 0;

    public DashboardAdapter(List<SwitchViewModel> items)
    {
        this.items = items;
    }

    public List<SwitchViewModel> getItems() {
        return items;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType)
    {
        RecyclerView.ViewHolder viewHolder = null;
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());

        switch (viewType)
        {
            case SWITCHITEM:
            {
                View switchView = inflater.inflate(R.layout.dashboard_switchitem, parent, false);
                viewHolder = new SwitchViewHolder(switchView);
            }
        }

        return viewHolder;
    }

    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        switch (holder.getItemViewType()) {
            case SWITCHITEM:
                SwitchViewHolder switchViewHolder = (SwitchViewHolder) holder;
                configureSwitchViewHolder(switchViewHolder, position);
                break;
        }
    }

    private void configureSwitchViewHolder(SwitchViewHolder switchViewHolder, final int position)
    {
        final SwitchViewModel item = (SwitchViewModel) items.get(position);
        if (item != null)
        {
            switchViewHolder.getTxtName().setText(item.getName());
            switchViewHolder.getTxtDesc().setText(item.getDescription());
            switchViewHolder.getTxtLastupdated().setText(item.getLastUpdated());

            switchViewHolder.getSwState().setChecked(item.getState() == SwitchState.ON);
            switchViewHolder.getSwState().setOnCheckedChangeListener(
                     new CompoundButton.OnCheckedChangeListener() {
                @Override
                public void onCheckedChanged(CompoundButton compoundButton, boolean b)
                {
                    SwitchState state = SwitchState.OFF;
                    if (b)
                    {
                        state = SwitchState.ON;
                    }

                    item.setState(state,true);

                }
            });
        }
    }

    @Override
    public int getItemViewType(int position) {
        if (items.get(position) instanceof SwitchViewModel)
        {
            return SWITCHITEM;
        }

        return -1;
    }

    @Override
    public int getItemCount()
    {
        return items.size();
    }
}
