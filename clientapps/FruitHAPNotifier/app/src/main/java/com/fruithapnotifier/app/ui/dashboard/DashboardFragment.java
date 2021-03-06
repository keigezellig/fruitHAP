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
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.AlertDialog;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.*;
import android.widget.Toast;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.*;
import com.fruithapnotifier.app.models.sensor.Sensor;
import com.fruithapnotifier.app.models.sensor.StatefulSensor;
import com.fruithapnotifier.app.models.sensor.button.Button;
import com.fruithapnotifier.app.models.sensor.image.ImageSensor;
import com.fruithapnotifier.app.models.sensor.image.ImageValueChangeEvent;
import com.fruithapnotifier.app.models.sensor.quantity.QuantitySensor;
import com.fruithapnotifier.app.models.sensor.quantity.QuantityValueChangeEvent;
import com.fruithapnotifier.app.models.sensor.switchy.Switch;
import com.fruithapnotifier.app.models.sensor.switchy.SwitchChangeEvent;
import com.fruithapnotifier.app.models.sensor.text.TextSensor;
import com.fruithapnotifier.app.models.sensor.text.TextValueChangeEvent;
import com.fruithapnotifier.app.persistence.ConfigurationRepository;
import com.fruithapnotifier.app.service.FruithapPubSubService;
import com.fruithapnotifier.app.service.configuration.DatabaseConfigurationLoader;
import com.fruithapnotifier.app.service.requestadapter.RestRequestAdapter;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SensorViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.button.ButtonViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.image.ImageViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.quantity.QuantityViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.switchy.SwitchViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.switchy.SwitchViewState;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.text.TextViewModel;
import com.fruithapnotifier.app.ui.helpers.UnitTextConverterFactory;
import com.fruithapnotifier.app.ui.main.FragmentCallbacks;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;

import java.util.ArrayList;
import java.util.List;

public class DashboardFragment extends Fragment
{
    private static final String TAG = DashboardFragment.class.getName();
    private String title;
    private FragmentCallbacks fragmentCallbacks;
    private DashboardAdapter adapter;
    private RecyclerView dashboardView;
    private ConfigurationRepository configurationRepository;
    private DatabaseConfigurationLoader configurationLoader;
    private Intent serviceIntent;
    private List<Sensor> sensors;

    public static DashboardFragment newInstance()
    {
        Bundle args = new Bundle();
        DashboardFragment fragment = new DashboardFragment();
        fragment.setArguments(args);
        return fragment;
    }

    public DashboardFragment()
    {
        sensors = new ArrayList<>();
    }

    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater)
    {
        super.onCreateOptionsMenu(menu, inflater);
    }

    @Override
    public void onPrepareOptionsMenu (Menu menu)
    {
        for (int i = 0; i < menu.size(); i++)
        {
            MenuItem item = menu.getItem(i);
            if (item.getItemId() == R.id.action_dashboard_refresh || item.getItemId() == R.id.action_values_refresh)
            {
                item.setEnabled(fragmentCallbacks.isConnectedToServer());
            }

            if (item.getItemId() == R.id.action_turnonservice)
            {
                item.setEnabled(!fragmentCallbacks.isConnectedToServer());
            }

            if (item.getItemId() == R.id.action_turnoffservice)
            {
                item.setEnabled(fragmentCallbacks.isConnectedToServer());
            }

        }

    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        int id = item.getItemId();

        if (id == R.id.action_dashboard_refresh)
        {
            configurationLoader.loadConfiguration();
            return true;
        }

        if (id == R.id.action_values_refresh)
        {
            for (Sensor sensor: configurationRepository.getSensors())
            {
                if (sensor instanceof StatefulSensor)
                {
                    ((StatefulSensor) sensor).requestUpdate();
                }
            }

            return true;
        }


        if (item.getItemId() == R.id.action_turnonservice)
        {

            getActivity().startService(serviceIntent);

            Toast.makeText(getActivity(), getString(R.string.notification_service_started), Toast.LENGTH_SHORT).show();
            return true;
        }

        if (item.getItemId() == R.id.action_turnoffservice)
        {
            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(
                    getActivity());

            // set title
            alertDialogBuilder.setTitle(getString(R.string.turn_off_notification_service));

            // set dialog message
            alertDialogBuilder
                    .setMessage(getString(R.string.notification_service_stopwarning))
                    .setCancelable(false)
                    .setPositiveButton(getString(R.string.caption_yes), new DialogInterface.OnClickListener()
                    {
                        public void onClick(DialogInterface dialog, int id)
                        {

                            getActivity().stopService(serviceIntent);

                        }
                    })
                    .setNegativeButton(getString(R.string.caption_no), new DialogInterface.OnClickListener()
                    {
                        public void onClick(DialogInterface dialog, int id)
                        {
                            // if this button is clicked, just close
                            // the dialog box and do nothing
                            dialog.cancel();
                        }
                    });

            // create alert dialog
            AlertDialog alertDialog = alertDialogBuilder.create();

            // show it
            alertDialog.show();
        }



        return super.onOptionsItemSelected(item);




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
                serviceIntent = new Intent(context, FruithapPubSubService.class);
                title = getString(R.string.title_dashboard);
                fragmentCallbacks = (FragmentCallbacks) activity;
                fragmentCallbacks.onSectionAttached(Constants.Section.DASHBOARD,title);
                EventBus.getDefault().register(this);
                configurationRepository = new ConfigurationRepository(context);
                configurationLoader = new DatabaseConfigurationLoader(configurationRepository, new RestRequestAdapter(context));
                setHasOptionsMenu(true);
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
        View view;

        if (fragmentCallbacks.isConnectedToServer()) {
            view = inflater.inflate(R.layout.dashboard_fragment, container, false);
            dashboardView = (RecyclerView) view.findViewById(R.id.dashboard);
            dashboardView.setHasFixedSize(true);
            LinearLayoutManager llm = new LinearLayoutManager(getActivity());
            llm.setOrientation(LinearLayoutManager.VERTICAL);

            dashboardView.setLayoutManager(llm);
            updateAdapter(true);
        }
        else
        {
            view = inflater.inflate(R.layout.dashboard_fragment_notconnected, container, false);
        }

        return view;
    }

    @Subscribe
    public void onConfigurationLoadError(ConfigurationLoadErrorEvent configurationLoadErrorEvent)
    {
        Toast.makeText(getActivity(), getString(R.string.configuration_load_error), Toast.LENGTH_SHORT).show();
        refreshDashboard();
        EventBus.getDefault().cancelEventDelivery(configurationLoadErrorEvent);
    }

    @Subscribe
    public void onConfigurationLoaded(ConfigurationLoadedEvent configurationLoadedEvent)
    {

        Toast.makeText(getActivity(), getString(R.string.configuration_successfully_loaded), Toast.LENGTH_SHORT).show();
        refreshDashboard();
        EventBus.getDefault().cancelEventDelivery(configurationLoadedEvent);
    }

    @Subscribe
    public void onConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
    {
        refreshDashboard();
        EventBus.getDefault().cancelEventDelivery(connectionChangedEvent);
    }


    private void refreshDashboard()
    {
        FragmentTransaction ft = getFragmentManager().beginTransaction();
        ft.detach(this).attach(this).commit();
        updateAdapter(true);

    }


    @Subscribe
    public void onSwitchStateChanged(SwitchChangeEvent event )
    {
        for (int i = 0; i < adapter.getItemCount(); i++)
        {
            SensorViewModel item = adapter.getItems().get(i);
            if (item.getName().equals(event.getSender().getName()) && item instanceof SwitchViewModel)
            {
                SwitchViewModel switchViewModel = (SwitchViewModel)item;
                Log.d(TAG,"Updating UI for switch: "+item.getName());
                switchViewModel.setState(SwitchViewState.values()[event.getSwitchState().ordinal()], false);
                DateTimeFormatter fmt = DateTimeFormat.forStyle("SM").withLocale(null);
                switchViewModel.setLastUpdated(event.getDate().toString(fmt));
                adapter.notifyItemChanged(i);
                break;
            }
        }
    }

    @Subscribe
    public void onQuantityValueChanged(QuantityValueChangeEvent event)
    {
        for (int i = 0; i < adapter.getItemCount(); i++)
        {
            SensorViewModel item = adapter.getItems().get(i);
            if (item.getName().equals(event.getSender().getName()) && item instanceof QuantityViewModel)
            {
                QuantityViewModel quantityViewModel = (QuantityViewModel)item;
                Log.d(TAG,"Updating UI for quanity value: "+item.getName());
                if (event.getValue() == null)
                {
                    quantityViewModel.setValue(Double.NaN);
                    quantityViewModel.setUnitText("");
                }
                else
                {
                    quantityViewModel.setValue(event.getValue().getValue());
                    String unitText = UnitTextConverterFactory.getUnitTextConverter(event.getValue().getType()).getUnitText(event.getValue().getUnit());
                    quantityViewModel.setUnitText(unitText);
                }
                DateTimeFormatter fmt = DateTimeFormat.forStyle("SM").withLocale(null);
                quantityViewModel.setLastUpdated(event.getDate().toString(fmt));
                adapter.notifyItemChanged(i);
                break;
            }
        }
    }

    @Subscribe
    public void onImageValueChanged(ImageValueChangeEvent event)
    {
        for (int i = 0; i < adapter.getItemCount(); i++)
        {
            SensorViewModel item = adapter.getItems().get(i);
            if (item.getName().equals(event.getSender().getName()) && item instanceof ImageViewModel)
            {
                ImageViewModel imageViewModel = (ImageViewModel)item;
                Log.d(TAG,"Updating UI for image value: "+item.getName());
                imageViewModel.setImageData(event.getValue());
                DateTimeFormatter fmt = DateTimeFormat.forStyle("SM").withLocale(null);
                imageViewModel.setLastUpdated(event.getDate().toString(fmt));
                adapter.notifyItemChanged(i);
                break;
            }
        }
    }

    @Subscribe
    public void onTextValueChanged(TextValueChangeEvent event)
    {
        for (int i = 0; i < adapter.getItemCount(); i++)
        {
            SensorViewModel item = adapter.getItems().get(i);
            if (item.getName().equals(event.getSender().getName()) && item instanceof TextViewModel)
            {
                TextViewModel textViewModel = (TextViewModel)item;
                Log.d(TAG,"Updating UI for text value: "+item.getName());
                textViewModel.setValue(event.getValue());
                DateTimeFormatter fmt = DateTimeFormat.forStyle("SM").withLocale(null);
                textViewModel.setLastUpdated(event.getDate().toString(fmt));
                adapter.notifyItemChanged(i);
                break;
            }
        }
    }



    private void updateAdapter(boolean shouldReload)
    {

        if (adapter == null || shouldReload)
        {
            if (sensors.size() > 0)
            {
                for (Sensor sensor : sensors)
                {
                    sensor.unregisterForEvents();
                }
                sensors.clear();
            }

            sensors = getSensorsFromDatasource();

            final List<SensorViewModel> viewItems = new ArrayList<>();
            for (Sensor sensor : sensors)
            {
                sensor.registerForEvents();
                SensorViewModel viewItem = convertToViewModel(sensor);
                if (viewItem != null)
                {
                    viewItems.add(viewItem);
                }
            }
            adapter = new DashboardAdapter(viewItems);
            dashboardView.setAdapter(adapter);

            getLatestValues(sensors);
        }
        else
        {
            if (adapter.getItemCount() > 0)
            {
                adapter.notifyDataSetChanged();
            }
        }
    }


    private SensorViewModel convertToViewModel(Sensor sensor)
    {
        if (sensor instanceof Switch)
        {
            Switch switchy = (Switch)sensor;
            return new SwitchViewModel(switchy.getName(),switchy.getDescription(), switchy.getCategory(),switchy.isReadOnly());
        }

        if (sensor instanceof Button)
        {
            Button button = (Button) sensor;
            return new ButtonViewModel(button.getName(),button.getDescription(),button.getCategory());
        }

        if (sensor instanceof QuantitySensor)
        {
            QuantitySensor quantitySensor = (QuantitySensor) sensor;
            return new QuantityViewModel(quantitySensor.getName(),quantitySensor.getDescription(),quantitySensor.getCategory());
        }

        if (sensor instanceof TextSensor)
        {
            TextSensor textSensor = (TextSensor) sensor;
            return new TextViewModel(textSensor.getName(),textSensor.getDescription(),textSensor.getCategory());
        }

        if (sensor instanceof ImageSensor)
        {
            ImageSensor imageSensor = (ImageSensor) sensor;
            return new ImageViewModel(imageSensor.getName(),imageSensor.getDescription(),imageSensor.getCategory());
        }



        return null;

    }

    private void getLatestValues(List<Sensor> sensors)
    {
        for (Sensor sensor: sensors)
        {
            if (sensor instanceof StatefulSensor)
            {
                ((StatefulSensor) sensor).requestUpdate();
            }
        }
    }

    private List<Sensor> getSensorsFromDatasource()
    {
        ConfigurationRepository datasource = new ConfigurationRepository(getActivity());
        List<Sensor> sensors = datasource.getSensors();
        return sensors;
    }

}
