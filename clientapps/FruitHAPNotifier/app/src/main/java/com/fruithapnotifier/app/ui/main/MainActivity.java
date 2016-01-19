package com.fruithapnotifier.app.ui.main;

import android.app.Activity;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.support.v4.widget.DrawerLayout;
import android.widget.TextView;
import android.widget.Toast;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.persistence.EventRepository;
import com.fruithapnotifier.app.service.FruithapNotificationService;
import com.fruithapnotifier.app.ui.alerts.AlertDetailActivity;
import com.fruithapnotifier.app.ui.alerts.AlertDetailFragment;
import com.fruithapnotifier.app.ui.alerts.AlertListFragment;

public class MainActivity extends AppCompatActivity
        implements NavigationDrawerFragment.NavigationDrawerCallbacks, AlertListFragment.Callbacks

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
    private Constants.MainScreenSection currentSection;
    private Intent serviceIntent;


    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        serviceIntent = new Intent(MainActivity.this, FruithapNotificationService.class);
        setContentView(R.layout.main_activity_main);
        mNavigationDrawerFragment = (NavigationDrawerFragment)
                getSupportFragmentManager().findFragmentById(R.id.navigation_drawer);
        mTitle = getTitle();

        // Set up the drawer.
        mNavigationDrawerFragment.setUp(
                R.id.navigation_drawer,
                (DrawerLayout) findViewById(R.id.drawer_layout));

        if (findViewById(R.id.alert_detail_container) != null)
        {
            // The detail container view will be present only in the
            // large-screen layouts (res/values-large and
            // res/values-sw600dp). If this view is present, then the
            // activity should be in two-pane mode.
            mTwoPane = true;

            // In two-pane mode, list items should be given the
            // 'activated' state when touched.
            ((AlertListFragment) getSupportFragmentManager()
                    .findFragmentById(R.id.alert_list))
                    .setActivateOnItemClick(true);
        }

    }

    /**
     * Callback method from {@link AlertListFragment.Callbacks}
     * indicating that the item with the given ID was selected.
     *
     * @param id
     */
    @Override
    public void onItemSelected(int id)
    {
        if (mTwoPane)
        {
            // In two-pane mode, show the detail view in this activity by
            // adding or replacing the detail fragment using a
            // fragment transaction.
            Bundle arguments = new Bundle();
            arguments.putInt(AlertDetailFragment.ARG_ITEM_ID, id);
            AlertDetailFragment fragment = new AlertDetailFragment();
            fragment.setArguments(arguments);
            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.alert_detail_container, fragment)
                    .commit();

        }
        else
        {
            // In single-pane mode, simply start the detail activity
            // for the selected item ID.
            Intent detailIntent = new Intent(this, AlertDetailActivity.class);
            detailIntent.putExtra(AlertDetailFragment.ARG_ITEM_ID, id);
            startActivity(detailIntent);
        }
    }

    @Override
    public void onNavigationDrawerItemSelected(int position)
    {

        FragmentManager fragmentManager = getSupportFragmentManager();
        switch (position)
        {
            case 0:
                //Home
                fragmentManager.beginTransaction()
                        .replace(R.id.container, new AlertListFragment())
                        .commit();
                break;
            case 1:
                //About
                fragmentManager.beginTransaction()
                        .replace(R.id.container, PlaceholderFragment.newInstance(position))
                        .commit();
                break;
        }
    }

    public void onSectionAttached(Constants.MainScreenSection section)
    {
        switch (section)
        {
            case ALERT_LIST:
                mTitle = getString(R.string.title_alertlist);
                break;
            case DASHBOARD:
                mTitle = getString(R.string.title_dashboard);
                break;

        }

        currentSection = section;


    }

    public void restoreActionBar()
    {
        ActionBar actionBar = getSupportActionBar();
        actionBar.setNavigationMode(ActionBar.NAVIGATION_MODE_STANDARD);
        actionBar.setDisplayShowTitleEnabled(true);
        actionBar.setTitle(mTitle);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        if (!mNavigationDrawerFragment.isDrawerOpen())
        {
            // Only show items in the action bar relevant to this screen
            // if the drawer is not showing. Otherwise, let the drawer
            // decide what to show in the action bar.

            if (currentSection == Constants.MainScreenSection.ALERT_LIST)
            {
                getMenuInflater().inflate(R.menu.menu_alertlist, menu);
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
        if (id == R.id.action_clear_list)
        {
            Toast.makeText(this, getString(R.string.clearing_list), Toast.LENGTH_SHORT).show();
            EventRepository repository = new EventRepository(this);
            repository.deleteAll();

            return true;
        }



        return super.onOptionsItemSelected(item);
    }

    /**
     * A placeholder fragment containing a simple view.
     */
    public static class PlaceholderFragment extends Fragment
    {
        /**
         * The fragment argument representing the section number for this
         * fragment.
         */
        private static final String ARG_SECTION_NUMBER = "section_number";

        /**
         * Returns a new instance of this fragment for the given section
         * number.
         */
        public static PlaceholderFragment newInstance(int sectionNumber)
        {
            PlaceholderFragment fragment = new PlaceholderFragment();
            Bundle args = new Bundle();
            args.putInt(ARG_SECTION_NUMBER, sectionNumber);
            fragment.setArguments(args);
            return fragment;
        }

        public PlaceholderFragment()
        {
        }

        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState)
        {
            View rootView = inflater.inflate(R.layout.main_fragment_main, container, false);
            TextView label = (TextView) rootView.findViewById(R.id.section_label);
            label.setText("This is section " + getArguments().getInt(ARG_SECTION_NUMBER));

            return rootView;
        }

        @Override
        public void onAttach(Activity activity)
        {
            super.onAttach(activity);
            ((MainActivity) activity).onSectionAttached(Constants.MainScreenSection.DASHBOARD);
        }
    }

}
