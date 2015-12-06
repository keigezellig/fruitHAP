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
import com.fruithapnotifier.app.ServiceControlActivity;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.persistence.SensorEventRepository;

import java.util.Date;

public class EventNotificationService extends Service
{
    private static String LOGTAG = "EventNotificationService";


    private EventNotificationTask eventNotificationTask;
    private NotificationManager notificationManager;
    private NotificationCompat.Builder notifyBuilder;

    private BroadcastReceiver eventNotificationReceiver;
    private SensorEventRepository datasource;


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
        notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        datasource = new SensorEventRepository(this);

        eventNotificationTask = new RabbitMQNotificationTask(this);
        eventNotificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.SENSOREVENT_ACTION))
                {
                    Date timestamp = new Date(intent.getLongExtra("timestamp", 0));
                    String sensorName = intent.getStringExtra("sensorName");
                    byte[] optionalData = intent.getByteArrayExtra("optionalData");
                    Log.d(LOGTAG, "Timestamp: " + timestamp.toString());
                    Log.d(LOGTAG, "Sensor name: " + sensorName);
                    Log.d(LOGTAG, "Optional data?: " + optionalData == null ? "yes" : "no");

                    NotificationCompat.Builder notifyBuilder = new NotificationCompat.Builder(context)
                            .setContentTitle("Incoming event!")
                            .setContentText(sensorName)
                            .setSmallIcon(R.mipmap.ic_launcher)
                            .setLights(Color.RED, 500, 500)
                            .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

                    notificationManager.notify(
                            Constants.INCOMING_EVENT_NOTIFICATIONID,
                            notifyBuilder.build());

                    // saveToDatabase(timestamp,sensorName,optionalData);

                }
            }
        };

        registerBroadcastListener();

        Log.d(LOGTAG,"Service created");

    }

    private void saveToDatabase(Date timestamp, String sensorName, byte[] optionalData)
    {
        datasource.open();
        datasource.createEvent(timestamp.getTime(),sensorName,optionalData);
        datasource.close();
    }

    @Override
    public void onDestroy()
    {
        unregisterBroadcastReceiver();
        notificationManager.cancel(Constants.SERVICE_STATE_NOTIFICATIONID);
        if (eventNotificationTask.getStatus() == AsyncTask.Status.RUNNING)
        {
            eventNotificationTask.cancel(true);
        }
        Log.d(LOGTAG, "Service stopped");
        //datasource.close();
        super.onDestroy();

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        PendingIntent stopServicePendingIntent = getStopServicePendingIntent();
        PendingIntent startServiceControlPendingIntent = getServiceControlPendingIntent();

        notifyBuilder = new NotificationCompat.Builder(this)
                .setContentTitle("FruitHap Notification Service")
                .setContentText("Service active")
                .setContentInfo("Service status")
                .setSmallIcon(R.mipmap.ic_launcher)
                .setContentIntent(startServiceControlPendingIntent)
                .setOngoing(true)
                .addAction(R.mipmap.ic_launcher,"Stop",stopServicePendingIntent)
                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

        notificationManager.notify(
                Constants.SERVICE_STATE_NOTIFICATIONID,
                notifyBuilder.build());



        String amqpUrl = "amqp://admin:admin@192.168.1.81";
        String exchangeName = "FruitHAP_PubSubExchange";
        String topic = "alerts";

        eventNotificationTask.execute(amqpUrl, exchangeName, topic);

        Log.d(LOGTAG, "Service started");
        return START_NOT_STICKY;
    }

    private PendingIntent getServiceControlPendingIntent()
    {
        Intent startServiceControlIntent = new Intent(this, ServiceControlActivity.class);
        startServiceControlIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);

        return PendingIntent.getActivity(
                this,
                0,
                startServiceControlIntent,
                PendingIntent.FLAG_UPDATE_CURRENT
        );
    }

    private PendingIntent getStopServicePendingIntent()
    {
        Intent stopServiceIntent = new Intent(Constants.STOP_ACTION);
        return PendingIntent.getBroadcast(this, -1, stopServiceIntent, PendingIntent.FLAG_UPDATE_CURRENT);
    }


    private void registerBroadcastListener()
    {
        LocalBroadcastManager.getInstance(this).registerReceiver(eventNotificationReceiver,new IntentFilter(Constants.SENSOREVENT_ACTION));
    }

    private void unregisterBroadcastReceiver()
    {
        LocalBroadcastManager.getInstance(this).unregisterReceiver(eventNotificationReceiver);
    }




}
