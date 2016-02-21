package com.fruithapnotifier.app.service;

import android.app.*;
import android.content.*;
import android.media.RingtoneManager;
import android.os.AsyncTask;
import android.os.IBinder;
import android.preference.PreferenceManager;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.AlertPriority;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.ui.main.MainActivity;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;

public class FruithapPubSubService extends Service
{
    private static String LOGTAG = "FruithapPubSubService";


    private FruithapNotificationTask fruithapNotificationTask;
    private NotificationManager notificationManager;
    private NotificationCompat.Builder notifyBuilder;

    private BroadcastReceiver eventNotificationReceiver;
    private AlertRepository datasource;
    private LocalBroadcastManager broadcastManager;
    private SharedPreferences preferences;


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
        datasource = new AlertRepository(this);
        preferences = PreferenceManager.getDefaultSharedPreferences(this);

        fruithapNotificationTask = new FruithapNotificationTask(this);


        eventNotificationReceiver = new BroadcastReceiver()
        {
            @Override
            public void onReceive(Context context, Intent intent)
            {

                if (intent.getAction().equals(Constants.ALERT_INSERTED))
                {
                    Alert alert = intent.getParcelableExtra(Constants.ALERT_DATA);
                    Log.d(LOGTAG, alert.toString());

                    if (alert.getNotificationText()!= null)
                    {
                        AlertPriority prio = alert.getNotificationPriority();
                        int color = PriorityHelpers.convertToColor(prio);

                        NotificationCompat.Builder notifyBuilder = new NotificationCompat.Builder(context)
                                .setContentTitle(alert.getNotificationText())
                                .setContentText(alert.getSensorName())
                                .setContentIntent(getEventDetailActivityIntent(alert.getId()))
                                .setSmallIcon(R.mipmap.ic_launcher)
                                .setAutoCancel(true)
                                .setSound(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION))
                                .setLights(color, 1000, 1000)
                                .setVisibility(NotificationCompat.VISIBILITY_PUBLIC);

                        notificationManager.notify(
                                Constants.INCOMING_ALERT_NOTIFICATION,
                                notifyBuilder.build());
                    }


                }

                if (intent.getAction().equals(Constants.INCOMING_ALERT))
                {
                    String topic = intent.getStringExtra(Constants.MQ_PUBSUB_TOPIC);
                    String message = intent.getStringExtra(Constants.MQ_PUBSUB_MESSAGE);
                    String alertTopic = preferences.getString("pref_server_alert_topic","alerts");
                    if (topic.equals(alertTopic) && !message.isEmpty())
                    {
                        try
                        {
                            JSONObject eventData = new JSONObject(message);
                            Alert alert = Alert.createAlertFromEventData(eventData);
                            if (alert != null)
                            {
                                datasource.insertAlert(alert);
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
        if (intent != null && intent.getAction() != null && intent.getAction().equals(Constants.STOP_ACTION))
        {
            stopSelf();
        }

        if (fruithapNotificationTask.getStatus() != AsyncTask.Status.RUNNING )
        {
            PendingIntent stopServicePendingIntent = getStopServicePendingIntent();
            PendingIntent startServiceControlActivityIntent = getServiceControlActivityIntent();

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


            ConnectionParameters parameters = getConnectionParameters();

            Log.d(getClass().getName(), parameters.toString());

            Log.d(getClass().getName(), "Starting task...");
            fruithapNotificationTask.execute(parameters);
        }
        else
        {
            Log.d(getClass().getName(), "Task already running");
        }

        return START_STICKY;
    }

    private ConnectionParameters getConnectionParameters()
    {

        ArrayList<String> topics = new ArrayList<String>();
        topics.add(preferences.getString("pref_server_alert_topic","alerts"));
        topics.add(preferences.getString("pref_server_event_topic","events"));

        return new ConnectionParameters(preferences.getString("pref_server_hostname","192.168.1.81"),
                Integer.valueOf(preferences.getString("pref_server_port","5672")),
                preferences.getString("pref_server_username","admin"),
                preferences.getString("pref_server_password","admin"),
                preferences.getString("pref_server_vhost","/"),
                preferences.getString("pref_server_event_exchange","FruitHAP_PubSubExchange"),
                topics);
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
        Intent stopServiceIntent = new Intent(this, FruithapPubSubService.class);
        stopServiceIntent.setAction(Constants.STOP_ACTION);
        return PendingIntent.getService(this, -1, stopServiceIntent, PendingIntent.FLAG_UPDATE_CURRENT);
    }

    private PendingIntent getEventDetailActivityIntent(int eventId)
    {
        Intent eventDetailActivityIntent = new Intent(this, MainActivity.class);
        eventDetailActivityIntent.putExtra(Constants.EXPANDED_ALERTID,eventId);
        TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
        stackBuilder.addParentStack(MainActivity.class);
        stackBuilder.addNextIntent(eventDetailActivityIntent);
        return stackBuilder.getPendingIntent(0, PendingIntent.FLAG_UPDATE_CURRENT);
    }


    private void registerBroadcastListener()
    {
        broadcastManager.registerReceiver(eventNotificationReceiver, new IntentFilter(Constants.INCOMING_ALERT));
        broadcastManager.registerReceiver(eventNotificationReceiver, new IntentFilter(Constants.ALERT_INSERTED));
    }

    private void unregisterBroadcastReceiver()
    {
        broadcastManager.unregisterReceiver(eventNotificationReceiver);
    }




}
