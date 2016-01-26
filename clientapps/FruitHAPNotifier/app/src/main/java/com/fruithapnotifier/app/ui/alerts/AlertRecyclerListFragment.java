package com.fruithapnotifier.app.ui.alerts;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.EventRepository;
import com.fruithapnotifier.app.ui.main.MainActivity;


public class AlertRecyclerListFragment extends Fragment
{

    private RecyclerView alertListView;
    private LocalBroadcastManager broadcastManager;
    private BroadcastReceiver onAlertDbChanged;
    private AlertRecycleAdapter adapter;

    public AlertRecyclerListFragment()
    {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        broadcastManager = LocalBroadcastManager.getInstance(getActivity());
        EventRepository datasource = new EventRepository(getActivity());
        adapter = new AlertRecycleAdapter(datasource.getAllAlerts(),datasource);

        onAlertDbChanged = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.ALERT_INSERTED))
                {
                    Alert alert = intent.getParcelableExtra("ALERTDATA");
                    adapter.insertItem(alert);
                    alertListView.smoothScrollToPosition(adapter.getItemCount() - 1);

                }

                if (intent.getAction().equals(Constants.ALERT_DELETED))
                {
                    Alert alert = intent.getParcelableExtra("ALERTDATA");
                    adapter.onItemDeleted(alert);
                }

                if (intent.getAction().equals(Constants.ALERTS_CLEARED))
                {
                    adapter.clearList();
                }

            }
        };


        registerBroadcastListener();
    }

    private void registerBroadcastListener()
    {
        IntentFilter alertDbChangedIntentFilter = new IntentFilter();
        alertDbChangedIntentFilter.addAction(Constants.ALERT_INSERTED);
        alertDbChangedIntentFilter.addAction(Constants.ALERT_DELETED);
        alertDbChangedIntentFilter.addAction(Constants.ALERTS_CLEARED);
        broadcastManager.registerReceiver(onAlertDbChanged, alertDbChangedIntentFilter);
    }

    private void unregisterBroadcastReceiver()
    {
        broadcastManager.unregisterReceiver(onAlertDbChanged);
    }




    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState)
    {
        super.onActivityCreated(savedInstanceState);
        alertListView = (RecyclerView) getActivity().findViewById(R.id.alertList);
        alertListView.setHasFixedSize(true);
        LinearLayoutManager llm = new LinearLayoutManager(getActivity());
        llm.setOrientation(LinearLayoutManager.VERTICAL);
        alertListView.setLayoutManager(llm);
        ItemTouchHelper.Callback callback = new AlertListTouchHelper(adapter);
        ItemTouchHelper helper = new ItemTouchHelper(callback);
        helper.attachToRecyclerView(alertListView);

        alertListView.setAdapter(adapter);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState)
    {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.alert_fragment_list, container, false);
    }


    @Override
    public void onAttach(Context context)
    {
        super.onAttach(context);

        Activity activity;

        if (context instanceof Activity)
        {
            activity = (Activity) context;



            if (activity instanceof MainActivity)
            {
                ((MainActivity) activity).onSectionAttached(Constants.MainScreenSection.ALERT_LIST);
            }
        }
    }

    @Override
    public void onDetach()
    {
        super.onDetach();

    }

}
