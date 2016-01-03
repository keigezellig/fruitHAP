package com.fruithapnotifier.app.ui;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.service.FruithapNotificationService;


/**
 * An activity representing a list of EventNotifications. This activity
 * has different presentations for handset and tablet-size devices. On
 * handsets, the activity presents a list of items, which when touched,
 * lead to a {@link EventNotificationDetailActivity} representing
 * item details. On tablets, the activity presents the list of items and
 * item details side-by-side using two vertical panes.
 * <p/>
 * The activity makes heavy use of fragments. The list of items is a
 * {@link EventNotificationListFragment} and the item details
 * (if present) is a {@link EventNotificationDetailFragment}.
 * <p/>
 * This activity also implements the required
 * {@link EventNotificationListFragment.Callbacks} interface
 * to listen for item selections.
 */
public class EventNotificationListActivity extends FragmentActivity
        implements EventNotificationListFragment.Callbacks {

    /**
     * Whether or not the activity is in two-pane mode, i.e. running on a tablet
     * device.
     */
    private boolean mTwoPane;
    private Intent serviceIntent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_eventnotification_list);

        if (findViewById(R.id.eventnotification_detail_container) != null) {
            // The detail container view will be present only in the
            // large-screen layouts (res/values-large and
            // res/values-sw600dp). If this view is present, then the
            // activity should be in two-pane mode.
            mTwoPane = true;

            // In two-pane mode, list items should be given the
            // 'activated' state when touched.
            ((EventNotificationListFragment) getSupportFragmentManager()
                    .findFragmentById(R.id.eventnotification_list))
                    .setActivateOnItemClick(true);
        }

    }

    @Override
    protected void onResume()
    {
        super.onResume();

       // serviceIntent = new Intent(EventNotificationListActivity.this, FruithapNotificationService.class);
        //startService(serviceIntent);

    }

    /**
     * Callback method from {@link EventNotificationListFragment.Callbacks}
     * indicating that the item with the given ID was selected.
     */
    @Override
    public void onItemSelected(String id) {
        if (mTwoPane) {
            // In two-pane mode, show the detail view in this activity by
            // adding or replacing the detail fragment using a
            // fragment transaction.
            Bundle arguments = new Bundle();
            arguments.putString(EventNotificationDetailFragment.ARG_ITEM_ID, id);
            EventNotificationDetailFragment fragment = new EventNotificationDetailFragment();
            fragment.setArguments(arguments);
            getSupportFragmentManager().beginTransaction()
                    .replace(R.id.eventnotification_detail_container, fragment)
                    .commit();

        } else {
            // In single-pane mode, simply start the detail activity
            // for the selected item ID.
            Intent detailIntent = new Intent(this, EventNotificationDetailActivity.class);
            detailIntent.putExtra(EventNotificationDetailFragment.ARG_ITEM_ID, id);
            startActivity(detailIntent);
        }
    }

    @Override
    protected void onDestroy()
    {
        super.onDestroy();
    }
}
