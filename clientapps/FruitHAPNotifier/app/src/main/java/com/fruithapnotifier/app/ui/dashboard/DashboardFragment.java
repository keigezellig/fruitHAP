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

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.models.sensor.Switch;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchViewModel;
import com.fruithapnotifier.app.ui.main.FragmentCallbacks;
import org.greenrobot.eventbus.EventBus;

import java.util.ArrayList;
import java.util.List;

public class DashboardFragment extends Fragment
{
    private String title;
    private FragmentCallbacks fragmentCallbacks;
    private DashboardAdapter adapter;
    private RecyclerView dashboardView;

    public static DashboardFragment newInstance()
    {
        Bundle args = new Bundle();
        DashboardFragment fragment = new DashboardFragment();
        fragment.setArguments(args);
        return fragment;
    }

    public DashboardFragment()
    {
    }

    @Override
    public void onAttach(Context context)
    {
        super.onAttach(context);

        Activity activity;

        if (context instanceof Activity)
        {
            activity = (Activity) context;
            try
            {
                title = getString(R.string.title_dashboard);
                fragmentCallbacks = (FragmentCallbacks) activity;
                fragmentCallbacks.onSectionAttached(Constants.Section.DASHBOARD,title);
            }
            catch (ClassCastException e)
            {
                throw new ClassCastException("Activity must implement FragmentCallbacks.");
            }
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState)
    {

        View view = inflater.inflate(R.layout.dashboard_fragment, container, false);
        dashboardView = (RecyclerView) view.findViewById(R.id.dashboard);
        dashboardView.setHasFixedSize(true);
        LinearLayoutManager llm = new LinearLayoutManager(getActivity());
        dashboardView.setLayoutManager(llm);

        updateAdapter();


        return view;
    }



    private void updateAdapter()
    {

        if (adapter == null)
        {
            final List<Switch> switches = getSwitchesFromDatasource();
            final List<SwitchViewModel> viewItems = new ArrayList<>();
            for (Switch switchy : switches)
            {
                SwitchViewModel viewItem = convertToViewModel(switchy);
                if (viewItem != null)
                {
                    viewItems.add(viewItem);
                }
            }
            adapter = new DashboardAdapter(viewItems);

            dashboardView.setAdapter(adapter);
            EventBus.getDefault().register(this);
        }
        else
        {
            if (adapter.getItemCount() > 0)
            {
                adapter.notifyDataSetChanged();
            }
        }
    }

    private SwitchViewModel convertToViewModel(Switch switchy)
    {
        return null;
    }

    private List<Switch> getSwitchesFromDatasource()
    {
        return null;
    }

}
