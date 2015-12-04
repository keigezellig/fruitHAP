package com.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.SystemClock;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.domain.Dummy;
import com.fruithapnotifier.app.domain.SensorEvent;

import java.io.Serializable;
import java.util.Date;
import java.util.Random;

/**
 * Created by developer on 11/28/15.
 */
public class EventNotificationTask extends AsyncTask<Void, Void, Void>
{
    private final Intent intent;
    private final LocalBroadcastManager broadcastManager;
    private static String SENSOREVENT_ACTION = "com.fruithapnotifier.app.action.SENSOR_EVENT";

    public EventNotificationTask(Context ctx) {
        this.broadcastManager = LocalBroadcastManager.getInstance(ctx);
        this.intent = new Intent(SENSOREVENT_ACTION);
    }

    @Override
    protected Void doInBackground(Void... voids)
    {
        Random rnd = new Random();
        while (!isCancelled())
        {
            intent.putExtra("timestamp",new Date());
            intent.putExtra("sensorName","Doorbell");
            intent.putExtra("optionalData", (String) null);

            broadcastManager.sendBroadcast(intent);
            SystemClock.sleep(5000);

        }

        Log.d("TASK", "Cancelled!");

        return null;
    }
}
