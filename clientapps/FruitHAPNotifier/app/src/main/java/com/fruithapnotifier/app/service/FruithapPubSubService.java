package com.fruithapnotifier.app.service;

import android.app.*;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.IBinder;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.widget.Toast;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.AlertPriority;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.EventRepository;
import com.fruithapnotifier.app.ui.alerts.AlertDetailActivity;
import com.fruithapnotifier.app.ui.alerts.AlertDetailFragment;
import com.fruithapnotifier.app.ui.main.MainActivity;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.json.JSONException;

public class FruithapPubSubService extends Service
{
    private static String LOGTAG = "FruithapPubSubService";


    private FruithapNotificationTask fruithapNotificationTask;
    private NotificationManager notificationManager;
    private NotificationCompat.Builder notifyBuilder;

    private BroadcastReceiver eventNotificationReceiver;
    private EventRepository datasource;
    private LocalBroadcastManager broadcastManager;


    public FruithapPubSubService()
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
        notificationManager = (NotificationManager) this.getSystemService(Context.NOTIFICATION_SERVICE);
        broadcastManager = LocalBroadcastManager.getInstance(this);
        datasource = new EventRepository(this);

        fruithapNotificationTask = new FruithapNotificationTask(this);
        eventNotificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.FRUITHAP_NOTIFICATION_ACTION))
                {
                    String topic = intent.getStringExtra(Constants.MQ_PUBSUB_TOPIC);
                    String message = intent.getStringExtra(Constants.MQ_PUBSUB_MESSAGE);

                    if (topic.equals("alerts") && !message.isEmpty())
                    {
                        try
                        {
                            Alert alert = datasource.insertAlert(message);

                            Log.d(LOGTAG, alert.toString());

                            if (alert.getNotificationText()!= null)
                            {
                                String messageText = alert.getNotificationText();
                                AlertPriority prio = alert.getNotificationPriority();
                                int color = PriorityHelpers.ConvertToColor(prio);

                                NotificationCompat.Builder notifyBuilder = new NotificationCompat.Builder(context)
                                        .setContentTitle(messageText)
                                        .setContentText(alert.getSensorName())
                                        .setContentIntent(getEventDetailActivityIntent(alert.getId()))
                                        .setSmallIcon(R.mipmap.ic_launcher)
                                        .setLights(color, 1000, 1000)
                                        .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

                                notificationManager.notify(
                                        Constants.INCOMING_EVENT_NOTIFICATIONID,
                                        notifyBuilder.build());
                            }


                        }
                        catch (JSONException e)
                        {
                            Log.e(LOGTAG, "Message error", e);
                        }
                    }
                }
            }
        };

        registerBroadcastListener();

        Log.d(LOGTAG, "Service created");

    }


    @Override
    public void onDestroy()
    {
        unregisterBroadcastReceiver();
        notificationManager.cancel(Constants.SERVICE_STATE_NOTIFICATIONID);
        if (fruithapNotificationTask.getStatus() == AsyncTask.Status.RUNNING)
        {
            fruithapNotificationTask.cancel(true);
        }

        Log.d(LOGTAG, "Service stopped");
        super.onDestroy();

    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        PendingIntent stopServicePendingIntent = getStopServicePendingIntent();
        PendingIntent startServiceControlActivityIntent = getServiceControlActivityIntent();
        final Bundle connectionParametersBundle = intent.getBundleExtra(Constants.MQ_CONNECTION_PARAMETERS);

        notifyBuilder = new NotificationCompat.Builder(this)
                .setContentTitle(getString(R.string.notification_service_name))
                .setSmallIcon(R.mipmap.strawberry)
                .setContentIntent(startServiceControlActivityIntent)
                .setOngoing(true)
                .addAction(R.drawable.ic_drawer, getString(R.string.turn_off_notification_service), stopServicePendingIntent)
                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

        notificationManager.notify(
                Constants.SERVICE_STATE_NOTIFICATIONID,
                notifyBuilder.build());



        String amqpUrl = "amqp://admin:admin@192.168.1.81";

        String exchangeName = "FruitHAP_PubSubExchange";
        String[] topics = new String[]{"alerts"};


        ConnectionParameters parameters = new ConnectionParameters(connectionParametersBundle.getString(Constants.MQ_HOST),
                connectionParametersBundle.getInt(Constants.MQ_PORT), connectionParametersBundle.getString(Constants.MQ_USERNAME), connectionParametersBundle.getString(Constants.MQ_PASSWORD),
                connectionParametersBundle.getString(Constants.MQ_VHOST), connectionParametersBundle.getString(Constants.MQ_PUBSUBEXCHANGE), connectionParametersBundle.getStringArrayList(Constants.MQ_PUBSUB_TOPICS_TO_SUBCRIBE));

        fruithapNotificationTask.execute(parameters);


        Toast.makeText(this, R.string.notification_service_started, Toast.LENGTH_SHORT).show();

        return START_NOT_STICKY;
    }

    private PendingIntent getServiceControlActivityIntent()
    {
        Intent startServiceControlIntent = new Intent(this, MainActivity.class);

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

    private PendingIntent getEventDetailActivityIntent(int eventId)
    {
        Intent eventDetailActivityIntent = new Intent(this, AlertDetailActivity.class);
        eventDetailActivityIntent.putExtra(AlertDetailFragment.ARG_ITEM_ID,eventId);
        eventDetailActivityIntent.putExtra(AlertDetailFragment.SHOULD_CLEAR_NOTIFICATION,true);
        TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
// Adds the back stack
        stackBuilder.addParentStack(AlertDetailActivity.class);
// Adds the Intent to the top of the stack
        stackBuilder.addNextIntent(eventDetailActivityIntent);
// Gets a PendingIntent containing the entire back stack
        return stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
    }


    private void registerBroadcastListener()
    {
        broadcastManager.registerReceiver(eventNotificationReceiver, new IntentFilter(Constants.FRUITHAP_NOTIFICATION_ACTION));
    }

    private void unregisterBroadcastReceiver()
    {
        broadcastManager.unregisterReceiver(eventNotificationReceiver);
    }

}
