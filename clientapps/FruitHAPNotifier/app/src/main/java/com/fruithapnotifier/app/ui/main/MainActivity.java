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

package com.fruithapnotifier.app.ui.main;

import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.FragmentManager;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.support.v4.widget.DrawerLayout;
import android.widget.Toast;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.ConfigurationLoader;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.models.sensor.Sensor;
import com.fruithapnotifier.app.models.sensor.StatefulSensor;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.persistence.ConfigurationRepository;
import com.fruithapnotifier.app.service.FruithapPubSubService;
import com.fruithapnotifier.app.service.configuration.DatabaseConfigurationLoader;
import com.fruithapnotifier.app.service.requestadapter.RestRequestAdapter;
import com.fruithapnotifier.app.ui.alerts.AlertListFragment;
import com.fruithapnotifier.app.ui.dashboard.DashboardFragment;

public class MainActivity extends AppCompatActivity
        implements NavigationDrawerCallbacks, FragmentCallbacks


{
    /**
     * Fragment managing the behaviors, interactions and presentation of the navigation drawer.
     */
    private NavigationDrawerFragment mNavigationDrawerFragment;
    private boolean mTwoPane;
    /**
     * Used to store the last screen title. For use in {@link #restoreActionBar()}.
     */
    private CharSequence mTitle;
    private Constants.Section currentSection;
    private Intent serviceIntent;
    private FragmentManager fragmentManager;
    private AlertRepository alertRepository;
    private ConfigurationRepository configurationRepository;


    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        fragmentManager = getSupportFragmentManager();
        serviceIntent = new Intent(MainActivity.this, FruithapPubSubService.class);
        startService(serviceIntent);

        setContentView(R.layout.main_activity_main);
        mNavigationDrawerFragment = (NavigationDrawerFragment) fragmentManager.findFragmentById(R.id.navigation_drawer);
        mTitle = getTitle();

        // Set up the drawer.
        mNavigationDrawerFragment.setUp(
                R.id.navigation_drawer,
                (DrawerLayout) findViewById(R.id.drawer_layout));

        alertRepository = new AlertRepository(this);
        configurationRepository = new ConfigurationRepository(this);


    }


    @Override
    public void onNavigationDrawerItemSelected(int position)
    {
        switch (position)
        {
            case 0:
                //Alert list
                fragmentManager.beginTransaction()
                        .replace(R.id.container, AlertListFragment.newInstance(-1))
                        .commit();
                break;
            case 1:
                //Dashboard
                fragmentManager.beginTransaction()
                        .replace(R.id.container, DashboardFragment.newInstance())
                        .commit();
                break;
        }
    }

    public void onSectionAttached(Constants.Section section, String title)
    {
        mTitle = title;
        currentSection = section;
    }

    public void restoreActionBar()
    {
        ActionBar actionBar = getSupportActionBar();
        actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
        actionBar.setDisplayShowTitleEnabled(true);
        actionBar.setTitle(mTitle);
    }

    public void updateTitle(String title)
    {
        mTitle = title;
        restoreActionBar();
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        if (!mNavigationDrawerFragment.isDrawerOpen())
        {
            // Only show items in the action bar relevant to this screen
            // if the drawer is not showing. Otherwise, let the drawer
            // decide what to show in the action bar.

            if (currentSection == Constants.Section.ALERT_LIST)
            {
                getMenuInflater().inflate(R.menu.menu_alertlist, menu);
            }

            if (currentSection == Constants.Section.DASHBOARD)
            {
                getMenuInflater().inflate(R.menu.menu_dashboard, menu);
            }

            restoreActionBar();
            return true;
        }
        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.



        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_alert_clear_list)
        {
            Toast.makeText(this, getString(R.string.clearing_list), Toast.LENGTH_SHORT).show();
            alertRepository.deleteAlerts();

            return true;
        }

        if (id == R.id.action_alert_mark_as_read)
        {
            alertRepository.markAllAsRead();
            return true;
        }

        if (id == R.id.action_dashboard_refresh)
        {
            ConfigurationLoader configurationLoader = new DatabaseConfigurationLoader(configurationRepository, new RestRequestAdapter(this));
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

        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onStart()
    {
        super.onStart();
        int expandedAlertId = getIntent().getIntExtra(Constants.EXPANDED_ALERTID,-1);
        if (expandedAlertId > -1)
        {
            fragmentManager.beginTransaction()
                    .replace(R.id.container, AlertListFragment.newInstance(expandedAlertId))
                    .commit();

            NotificationManager notificationManager = (NotificationManager)getSystemService(Context.NOTIFICATION_SERVICE);
            notificationManager.cancel(Constants.INCOMING_ALERT_NOTIFICATION);
        }
    }


}
