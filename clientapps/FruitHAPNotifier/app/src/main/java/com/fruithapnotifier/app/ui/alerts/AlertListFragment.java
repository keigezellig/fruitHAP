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
import com.bignerdranch.expandablerecyclerview.Model.ParentListItem;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemDetailViewModel;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import com.fruithapnotifier.app.ui.main.FragmentCallbacks;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;

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

            if (alert.getOptionalData() != null && !alert.getOptionalData().isNull("$type") && (alert.getOptionalData().getString("$type").contains("Byte")))
            {
                String imageString = alert.getOptionalData().getString("$value");
                image = Base64.decode(imageString, Base64.DEFAULT);
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
                    Alert alert = intent.getParcelableExtra("ALERTDATA");
                    AlertListItemViewModel item = convertToViewModel(alert);
                    if (item != null)
                    {
                        adapterItems.add(0,item);
                        adapter.notifyParentItemInserted(0);
                    }
                }

                if (intent.getAction().equals(Constants.ALERT_UPDATED))
                {
                    Alert alert = intent.getParcelableExtra("ALERTDATA");

                    int position = adapterItems.indexOf(adapter.findAlertByIdInAdapter(alert.getId()));
                    if (position > -1)
                    {
                        AlertListItemViewModel newItem = convertToViewModel(alert);
                        adapterItems.set(position,newItem);
                        adapter.notifyParentItemChanged(position);
                    }

                }

                if (intent.getAction().equals(Constants.ALERT_DELETED))
                {
                    Alert alert = intent.getParcelableExtra("ALERTDATA");

                    int position = adapterItems.indexOf(adapter.findAlertByIdInAdapter(alert.getId()));
                    if (position > -1)
                    {
                        adapterItems.remove(position);
                        adapter.notifyParentItemRemoved(position);
                        NotificationManager notificationManager = (NotificationManager)getActivity().getSystemService(Context.NOTIFICATION_SERVICE);
                        notificationManager.cancel(Constants.INCOMING_ALERT_NOTIFICATION);
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
                    AlertRepository repos = new AlertRepository(getActivity());
                    Alert toBeUpdated = repos.getAlertById(id);
                    toBeUpdated.setRead(true);
                    repos.updateAlert(toBeUpdated);
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
        int alertIdToBeExpanded = getArguments().getInt(ARG_EXPANDED_ALERTID,-1);

        if (alertIdToBeExpanded > -1)
        {
            int pos = adapter.getParentItemList().indexOf(adapter.findAlertByIdInAdapter(alertIdToBeExpanded));
            if (pos != -1)
            {
                adapter.expandParent(pos);
                alertListView.scrollToPosition(pos);
            }
        }
        else
        {
            adapter.collapseAllParents();
        }

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
                mCallbacks = (FragmentCallbacks) activity;
                mCallbacks.onSectionAttached(Constants.Section.ALERT_LIST);
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
