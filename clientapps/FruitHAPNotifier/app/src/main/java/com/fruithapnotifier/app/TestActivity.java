package com.fruithapnotifier.app;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import com.fruithapnotifier.app.service.NotificationService;


public class TestActivity extends ActionBarActivity {

    private Intent serviceIntent;
    private BroadcastReceiver notificationReceiver;

    public TestActivity()
    {

    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        serviceIntent = new Intent(TestActivity.this, NotificationService.class);
        notificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                String message = intent.getStringExtra("message");
                Log.d("receiver", "Got message: " + message);

            }
        };

        setContentView(R.layout.activity_test);

    }

    @Override
    protected void onResume()
    {
        super.onResume();
        registerBroadcastListener();
    }

    private void registerBroadcastListener()
    {
        LocalBroadcastManager.getInstance(this).registerReceiver(notificationReceiver,new IntentFilter("ACTION_FRUITHAP_NOTIFICATION"));
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_test, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onPause()
    {
        unregisterBroadcastReceiver();
        super.onPause();
    }

    private void unregisterBroadcastReceiver()
    {
        LocalBroadcastManager.getInstance(this).unregisterReceiver(notificationReceiver);
    }

    public void startNotificationService (View view)
    {
        startService(serviceIntent);
    }

    public void stopNotificationService (View view)
    {
        stopService(serviceIntent);
    }
}
