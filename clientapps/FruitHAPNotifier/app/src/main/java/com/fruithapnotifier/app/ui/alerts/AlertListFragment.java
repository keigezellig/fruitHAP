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

import android.app.Activity;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.bignerdranch.expandablerecyclerview.Adapter.ExpandableRecyclerAdapter;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.models.alert.Alert;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemDetailViewModel;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import com.fruithapnotifier.app.ui.main.FragmentCallbacks;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;


public class AlertListFragment extends Fragment
{

    private static final String ARG_EXPANDED_ALERTID = "expanded_alertId";

    private RecyclerView alertListView;
    private LocalBroadcastManager broadcastManager;
    private BroadcastReceiver onAlertDbChanged;
    private AlertListAdapter adapter;
    private FragmentCallbacks mCallbacks;
    private String title;

    public static AlertListFragment newInstance(int alertIdThatShouldBeExpanded)
    {
        Bundle args = new Bundle();
        args.putInt(ARG_EXPANDED_ALERTID,alertIdThatShouldBeExpanded);
        AlertListFragment fragment = new AlertListFragment();
        fragment.setArguments(args);
        return fragment;
    }

    public AlertListFragment()
    {
        // Required empty public constructor
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
    }

    private AlertListItemViewModel convertToViewModel(Alert alert)
    {
        try
        {
            int id = alert.getId();
            String timestamp = DateTimeFormat.forStyle("SL").print(alert.getTimestamp());
            String sensorName = alert.getSensorName();
            String notificationText = alert.getNotificationText();
            String priorityText = getString(PriorityHelpers.getTextResource(alert.getNotificationPriority()));
            int priorityColor = PriorityHelpers.convertToColor(alert.getNotificationPriority());
            byte[] image = null;

            if (alert.getOptionalData() != null)
            {
                JSONObject content = alert.getOptionalData().optJSONObject("Content");
                if (content != null)
                {
                    if (content.has("$type") && content.getString("$type").contains("Byte"))
                    {
                        String imageString = content.getString("$value");
                        image = Base64.decode(imageString, Base64.DEFAULT);
                    }
                }

            }
            AlertListItemViewModel result = new AlertListItemViewModel(id, timestamp, notificationText,priorityText,priorityColor,alert.isRead());
            ArrayList<AlertListItemDetailViewModel> childList = new ArrayList<>();
            childList.add(new AlertListItemDetailViewModel(timestamp,sensorName,notificationText,priorityText,priorityColor,image));
            result.setChildItemList(childList);
            return result;

        }
        catch (JSONException e)
        {
            Log.e(getClass().getName(),"Invalid format in record");
            return null;
        }

    }

    private void registerBroadcastReceiver()
    {
        broadcastManager = LocalBroadcastManager.getInstance(getActivity());
        IntentFilter alertDbChangedIntentFilter = new IntentFilter();
        alertDbChangedIntentFilter.addAction(Constants.ALERT_INSERTED);
        alertDbChangedIntentFilter.addAction(Constants.ALERT_DELETED);
        alertDbChangedIntentFilter.addAction(Constants.ALERTS_CLEARED);
        alertDbChangedIntentFilter.addAction(Constants.ALERT_UPDATED);
        alertDbChangedIntentFilter.addAction(Constants.ALERTS_RANGEUPDATED);
        broadcastManager.registerReceiver(onAlertDbChanged, alertDbChangedIntentFilter);
    }

    private void unregisterBroadcastReceiver()
    {
        broadcastManager.unregisterReceiver(onAlertDbChanged);
    }

    private BroadcastReceiver setupBroadcastReceiver()
    {

        BroadcastReceiver receiver = new BroadcastReceiver() {
            final List<AlertListItemViewModel> adapterItems = (List<AlertListItemViewModel>)adapter.getParentItemList();



            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.ALERT_INSERTED))
                {
                    Alert alert = intent.getParcelableExtra(Constants.ALERT_DATA);
                    AlertListItemViewModel item = convertToViewModel(alert);
                    if (item != null)
                    {
                        adapterItems.add(0,item);
                        adapter.notifyParentItemInserted(0);
                        alertListView.scrollToPosition(0);
                        updateCounters();
                    }

                }

                if (intent.getAction().equals(Constants.ALERT_UPDATED))
                {
                    Alert alert = intent.getParcelableExtra(Constants.ALERT_DATA);

                    int position = adapterItems.indexOf(adapter.findAlertByIdInAdapter(alert.getId()));
                    if (position > -1)
                    {
                        AlertListItemViewModel newItem = convertToViewModel(alert);
                        adapterItems.set(position,newItem);
                        adapter.notifyParentItemRangeChanged(0, adapterItems.size());
                        updateCounters();

                    }

                }

                if (intent.getAction().equals(Constants.ALERT_DELETED))
                {
                    Alert alert = intent.getParcelableExtra(Constants.ALERT_DATA);

                    int position = adapterItems.indexOf(adapter.findAlertByIdInAdapter(alert.getId()));
                    if (position > -1)
                    {
                        adapterItems.remove(position);
                        adapter.notifyParentItemRemoved(position);
                        NotificationManager notificationManager = (NotificationManager)getActivity().getSystemService(Context.NOTIFICATION_SERVICE);
                        notificationManager.cancel(Constants.INCOMING_ALERT_NOTIFICATION);
                        updateCounters();
                    }
                }

                if (intent.getAction().equals(Constants.ALERTS_CLEARED))
                {
                    if (adapter.getParentItemList().size() > 0)
                    {
                        int itemsToBeRemoved = adapterItems.size();
                        adapterItems.clear();
                        adapter.notifyParentItemRangeRemoved(0, itemsToBeRemoved);
                        NotificationManager notificationManager = (NotificationManager)getActivity().getSystemService(Context.NOTIFICATION_SERVICE);
                        notificationManager.cancel(Constants.INCOMING_ALERT_NOTIFICATION);
                        updateCounters();
                    }
                }

                if (intent.getAction().equals(Constants.ALERTS_RANGEUPDATED))
                {
                    if (adapterItems.size() > 0)
                    {
                        ArrayList<Alert> updated = intent.getParcelableArrayListExtra(Constants.ALERT_RANGEDATA);

                        for (Alert alert: updated)
                        {
                            int position = adapterItems.indexOf(adapter.findAlertByIdInAdapter(alert.getId()));
                            AlertListItemViewModel newItem = convertToViewModel(alert);
                            adapterItems.set(position,newItem);
                        }

                        adapter.notifyParentItemRangeChanged(0, adapterItems.size());
                        NotificationManager notificationManager = (NotificationManager) getActivity().getSystemService(Context.NOTIFICATION_SERVICE);
                        notificationManager.cancel(Constants.INCOMING_ALERT_NOTIFICATION);
                        updateCounters();
                    }
                }

            }
        };

        return receiver;
    }


    private void updateAdapter()
    {

        if (adapter == null)
        {
            final List<Alert> alerts = getAlertsFromDatasource();
            final List<AlertListItemViewModel> viewItems = new ArrayList<>();
            for (Alert alert : alerts)
            {
                AlertListItemViewModel viewItem = convertToViewModel(alert);
                if (viewItem != null)
                {
                    viewItems.add(viewItem);
                }
            }
            adapter = new AlertListAdapter(getActivity(), viewItems);
            adapter.setExpandCollapseListener(new ExpandableRecyclerAdapter.ExpandCollapseListener() {
                @Override
                public void onListItemExpanded(int i)
                {
                    AlertListItemViewModel item = (AlertListItemViewModel)adapter.getParentItemList().get(i);
                    int id = item.getId();
                    updateReadStatus(id);
                }

                @Override
                public void onListItemCollapsed(int i) {

                }
            });
            alertListView.setAdapter(adapter);
            ItemTouchHelper.Callback callback = new AlertListTouchHelper(adapter,getActivity());
            ItemTouchHelper helper = new ItemTouchHelper(callback);
            helper.attachToRecyclerView(alertListView);
            onAlertDbChanged = setupBroadcastReceiver();
            registerBroadcastReceiver();
        }
        else
        {
            if (adapter.getParentItemList().size() > 0)
            {
                adapter.notifyParentItemRangeChanged(0,adapter.getParentItemList().size());
            }
        }
        updateCounters();

        int alertIdToBeExpanded = getArguments().getInt(ARG_EXPANDED_ALERTID,-1);

        if (alertIdToBeExpanded > -1)
        {
            int pos = adapter.getParentItemList().indexOf(adapter.findAlertByIdInAdapter(alertIdToBeExpanded));
            if (pos != -1)
            {
                AlertListItemViewModel item = (AlertListItemViewModel)adapter.getParentItemList().get(pos);
                int id = item.getId();
                updateReadStatus(id);
                adapter.expandParent(pos);
                alertListView.scrollToPosition(pos);
            }
        }
        else
        {
            adapter.collapseAllParents();
        }

    }

    private void updateReadStatus(int id)
    {
        AlertRepository repos = new AlertRepository(getActivity());
        Alert toBeUpdated = repos.getAlertById(id);
        if (!toBeUpdated.isRead())
        {
            toBeUpdated.setRead(true);
            repos.updateAlert(toBeUpdated);
        }

    }

    private void updateCounters()
    {
        int unReadAlerts = getUnreadAlerts();

        String title = getString(R.string.title_alertlist) + " (" + unReadAlerts + ")";
        mCallbacks.updateTitle(title);
    }

    private int getUnreadAlerts()
    {
        int number = 0;
        List<AlertListItemViewModel> viewItems = (List<AlertListItemViewModel>)adapter.getParentItemList();
        for (AlertListItemViewModel item : viewItems)
        {
            if (!item.isRead())
            {
                number++;
            }
        }
        return number;
    }


    @Override
    public void onResume()
    {
        super.onResume();
        updateAdapter();
    }

    private List<Alert> getAlertsFromDatasource()
    {
        AlertRepository datasource = new AlertRepository(getActivity());
        List<Alert> alerts = datasource.getAllAlerts();
        return alerts;
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState)
    {

        View view = inflater.inflate(R.layout.alert_fragment_list, container, false);
        alertListView = (RecyclerView) view.findViewById(R.id.alertList);
        alertListView.setHasFixedSize(true);
        LinearLayoutManager llm = new LinearLayoutManager(getActivity());
        alertListView.setLayoutManager(llm);

        updateAdapter();


        return view;
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
                title = getString(R.string.title_alertlist);
                mCallbacks = (FragmentCallbacks) activity;
                mCallbacks.onSectionAttached(Constants.Section.ALERT_LIST,title);
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
        unregisterBroadcastReceiver();

    }




}
