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
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.models.sensor.Switch;
import com.fruithapnotifier.app.models.sensor.SwitchChangeEvent;
import com.fruithapnotifier.app.models.sensor.SwitchState;
import com.fruithapnotifier.app.persistence.ConfigurationRepository;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchViewModel;
import com.fruithapnotifier.app.ui.main.FragmentCallbacks;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;

import java.util.ArrayList;
import java.util.List;

public class DashboardFragment extends Fragment
{
    private static final String TAG = DashboardFragment.class.getName();
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
    public void onDetach()
    {
        super.onDetach();
        EventBus.getDefault().unregister(this);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState)
    {

        View view = inflater.inflate(R.layout.dashboard_fragment, container, false);
        dashboardView = (RecyclerView) view.findViewById(R.id.dashboard);
        dashboardView.setHasFixedSize(true);
        LinearLayoutManager llm = new LinearLayoutManager(getActivity());
        llm.setOrientation(LinearLayoutManager.VERTICAL);

        dashboardView.setLayoutManager(llm);

        updateAdapter();


        return view;
    }


    @Subscribe
    public void onStateChanged(SwitchChangeEvent event )
    {
        for (int i = 0; i < adapter.getItemCount(); i++)
        {
            SwitchViewModel item = adapter.getItems().get(i);
            if (item.getName().equals(event.getSender().getName()))
            {
                Log.d(TAG,"Updating UI for switch: "+item.getName());
                item.setState(com.fruithapnotifier.app.ui.dashboard.viewmodels.SwitchState.values()[event.getSwitchState().ordinal()], false);
                DateTimeFormatter fmt = DateTimeFormat.forStyle("SM").withLocale(null);
                item.setLastUpdated(event.getDate().toString(fmt));
                adapter.notifyItemChanged(i);
            }
        }
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
            updateSwitches(switches);
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
        return new SwitchViewModel(switchy.getName(),switchy.getDescription(),switchy);
    }

    private void updateSwitches(List<Switch> switches)
    {
        for (Switch switchy: switches)
        {
            switchy.requestUpdate();
        }
    }

    private List<Switch> getSwitchesFromDatasource()
    {
        ConfigurationRepository datasource = new ConfigurationRepository(getActivity());
        List<Switch> switches = datasource.getSwitches();
        return switches;
    }

}
