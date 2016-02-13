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
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.EventRepository;
import com.fruithapnotifier.app.ui.alerts.viewmodels.AlertListItemViewModel;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import com.fruithapnotifier.app.ui.main.MainActivity;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;

import java.util.ArrayList;
import java.util.List;


public class AlertRecyclerListFragment extends Fragment
{

    private RecyclerView alertListView;
    private LocalBroadcastManager broadcastManager;
    private BroadcastReceiver onAlertDbChanged;
    private AlertExpandableRecycleAdapter adapter;

    public AlertRecyclerListFragment()
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
            String timestamp = DateTimeFormat.forStyle("SL").print(alert.getTimestamp());
            String sensorName = alert.getSensorName();
            String notificationText = alert.getNotificationText();
            String priorityText = getActivity().getString(PriorityHelpers.getTextResource(alert.getNotificationPriority()));
            int priorityColor = PriorityHelpers.convertToColor(alert.getNotificationPriority());
            byte[] image = null;

            if (alert.getOptionalData() != null && !alert.getOptionalData().isNull("$type") && (alert.getOptionalData().getString("$type").contains("Byte")))
            {
                String imageString = alert.getOptionalData().getString("$value");
                image = Base64.decode(imageString, Base64.DEFAULT);
            }
            AlertListItemViewModel result = new AlertListItemViewModel(timestamp, sensorName, notificationText,priorityText,priorityColor, image);
            return result;
        }
        catch (JSONException e)
        {
            Log.e(getClass().getName(),"Invalid format in record");
            return null;
        }

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

        broadcastManager = LocalBroadcastManager.getInstance(getActivity());
        EventRepository datasource = new EventRepository(getActivity());
        List<Alert> alerts = datasource.getAllAlerts();
        final List<AlertListItemViewModel> viewItems = new ArrayList<AlertListItemViewModel>();
        for (Alert alert : alerts)
        {
            AlertListItemViewModel viewItem = convertToViewModel(alert);
            if (viewItem != null)
            {
                viewItems.add(viewItem);
            }
        }

        adapter = new AlertExpandableRecycleAdapter(getActivity(),viewItems);

        onAlertDbChanged = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.ALERT_INSERTED))
                {
                    if ((getActivity() instanceof MainActivity ))
                    {
                        Alert alert = intent.getParcelableExtra("ALERTDATA");
                        AlertListItemViewModel item = convertToViewModel(alert);
                        if (item != null)
                        {
                            viewItems.add(item);
                            adapter.notifyParentItemInserted(viewItems.size() - 1);
                        }
                    }

                }

//                if (intent.getAction().equals(Constants.ALERT_DELETED))
//                {
//                    Alert alert = intent.getParcelableExtra("ALERTDATA");
//                    adapter.onItemDeleted(alert);
//                }

                if (intent.getAction().equals(Constants.ALERTS_CLEARED))
                {
                    adapter.notifyParentItemRangeRemoved(0,viewItems.size());
                    viewItems.clear();

                }

            }
        };


        registerBroadcastListener();

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
