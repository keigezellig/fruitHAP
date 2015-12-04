package com.fruithapnotifier.app.service;

import android.app.*;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.IBinder;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.R;

import java.util.Date;

public class EventNotificationService extends Service
{
    private static String LOGTAG = "EventNotificationService";
    private static String SENSOREVENT_ACTION = "com.fruithapnotifier.app.action.SENSOR_EVENT";

    private static int SERVICE_STATE_NOTIFICATIONID = 1;
    private static int INCOMING_EVENT_NOTIFICATIONID = 2;

    private static String START_ACTION = "com.fruithapnotifier.app.action.START_SERVICE";
    private static String STOP_ACTION = "com.fruithapnotifier.app.action.STOP_SERVICE";

    private EventNotificationTask eventNotificationTask;
    private NotificationManager notificationManager;
    private NotificationCompat.Builder notifyBuilder;

    private BroadcastReceiver eventNotificationReceiver;


    public EventNotificationService()
    {
    }

    @Override
    public IBinder onBind(Intent intent)
    {
        return null;
    }

    @Override
    public void onCreate()
    {
        super.onCreate();
        eventNotificationTask = new EventNotificationTask(this);

        eventNotificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {

                String value = intent.getStringExtra("itemValue");
                Date timestamp =  new Date(intent.getLongExtra("itemTimestamp",0));
                Log.d(LOGTAG,"Id: "+id);
                Log.d(LOGTAG,"Timestamp: "+timestamp.toString());
                Log.d(LOGTAG,"Value: "+value);

                NotificationCompat.Builder notifyBuilder = new NotificationCompat.Builder(context)
                        .setContentTitle("Incoming event!")
                        .setContentText(intent.getStringExtra("itemValue"))
                        .setContentInfo(intent.getStringExtra("itemId"))
                        .setSmallIcon(R.mipmap.ic_launcher)
                        .setLights(Color.RED,100,100)
                        .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

                notificationManager.notify(
                        INCOMING_EVENT_NOTIFICATIONID,
                        notifyBuilder.build());

            }
        };

        registerBroadcastListener();

        Log.d(LOGTAG,"Service created");

    }

    @Override
    public void onDestroy()
    {
        unregisterBroadcastReceiver();
        notificationManager.cancel(SERVICE_STATE_NOTIFICATIONID);
        eventNotificationTask.cancel(true);
        Log.d(LOGTAG, "Service stopped");
        super.onDestroy();

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        Log.d(LOGTAG, "Service started");

        notificationManager =
                (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

/*        // Creates an explicit intent for an Activity in your app
        Intent notifyIntent = new Intent(this, ServiceControlActivity.class);

// Sets the Activity to start in a new, empty task
        notifyIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
                | Intent.FLAG_ACTIVITY_CLEAR_TASK);
// Creates the PendingIntent
        PendingIntent notifyPendingIntent =
                PendingIntent.getActivity(
                        this,
                        0,
                        notifyIntent,
                        PendingIntent.FLAG_UPDATE_CURRENT
                );*/

        Intent stopServiceIntent = new Intent(STOP_ACTION);
        PendingIntent pendingIntentStop = PendingIntent.getBroadcast(this, -1, stopServiceIntent, PendingIntent.FLAG_UPDATE_CURRENT);


        notifyBuilder = new NotificationCompat.Builder(this)
                .setContentTitle("FruitHap Notification Service")
                .setContentText("Service active")
                .setContentInfo("Service status")
                .setSmallIcon(R.mipmap.ic_launcher)
                .setOngoing(true)
                .addAction(R.mipmap.ic_launcher,"Stop",pendingIntentStop)
                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);


        notificationManager.notify(
                SERVICE_STATE_NOTIFICATIONID,
                notifyBuilder.build());


        if (eventNotificationTask.getStatus() != AsyncTask.Status.RUNNING)
        {
            eventNotificationTask.execute((Void) null);
        }
        return START_NOT_STICKY;
    }


    private void registerBroadcastListener()
    {
        LocalBroadcastManager.getInstance(this).registerReceiver(eventNotificationReceiver,new IntentFilter(SENSOREVENT_ACTION));
    }

    private void unregisterBroadcastReceiver()
    {
        LocalBroadcastManager.getInstance(this).unregisterReceiver(eventNotificationReceiver);
    }




}
