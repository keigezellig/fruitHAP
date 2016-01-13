package com.fruithapnotifier.app.service;

import android.app.*;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.AsyncTask;
import android.os.IBinder;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.Priority;
import com.fruithapnotifier.app.domain.SensorEvent;
import com.fruithapnotifier.app.ui.alerts.AlertDetailActivity;
import com.fruithapnotifier.app.ui.alerts.AlertDetailFragment;
import com.fruithapnotifier.app.ui.main.MainActivity;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.persistence.SensorEventRepository;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.json.JSONException;

public class FruithapNotificationService extends Service
{
    private static String LOGTAG = "FruithapNotificationService";


    private FruithapNotificationTask fruithapNotificationTask;
    private NotificationManager notificationManager;
    private NotificationCompat.Builder notifyBuilder;

    private BroadcastReceiver eventNotificationReceiver;
    private SensorEventRepository datasource;


    public FruithapNotificationService()
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

        datasource = new SensorEventRepository(this);

        fruithapNotificationTask = new FruithapNotificationTask(this);
        eventNotificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {
                if (intent.getAction().equals(Constants.FRUITHAP_NOTIFICATION_ACTION))
                {
                    String message = intent.getStringExtra(Constants.MQ_PUBSUB_MESSAGE);

                    if (!message.isEmpty())
                    {
                        try
                        {
                            SensorEvent event = datasource.createEvent(message);

                            Log.d(LOGTAG, event.toString());

                            if (event.getNotificationText()!= null)
                            {
                                String messageText = event.getNotificationText();
                                Priority prio = event.getNotificationPriority();
                                int color = PriorityHelpers.ConvertToColor(prio);

                                NotificationCompat.Builder notifyBuilder = new NotificationCompat.Builder(context)
                                        .setContentTitle(messageText)
                                        .setContentText(event.getSensorName())
                                        .setContentIntent(getEventDetailActivityIntent(event.getId()))
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

        notifyBuilder = new NotificationCompat.Builder(this)
                .setContentTitle("FruitHap Notification Service")
                .setContentText("Service active")
                .setContentInfo("Service status")
                .setSmallIcon(R.mipmap.strawberry)
                .setContentIntent(startServiceControlActivityIntent)
                .setOngoing(true)
                .addAction(R.mipmap.strawberry, "Stop", stopServicePendingIntent)
                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

        notificationManager.notify(
                Constants.SERVICE_STATE_NOTIFICATIONID,
                notifyBuilder.build());


        String amqpUrl = "amqp://admin:admin@192.168.1.81";
        String exchangeName = "FruitHAP_PubSubExchange";
        String topic = "alerts";

        fruithapNotificationTask.execute(amqpUrl, exchangeName, topic);

        Log.d(LOGTAG, "Service started");
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
        LocalBroadcastManager.getInstance(this).registerReceiver(eventNotificationReceiver, new IntentFilter(Constants.FRUITHAP_NOTIFICATION_ACTION));
    }

    private void unregisterBroadcastReceiver()
    {
        LocalBroadcastManager.getInstance(this).unregisterReceiver(eventNotificationReceiver);
    }

}
