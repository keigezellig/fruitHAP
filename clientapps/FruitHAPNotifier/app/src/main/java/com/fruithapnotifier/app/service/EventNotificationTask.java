package com.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.SystemClock;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

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
        int counter=0;
        while (!isCancelled())
        {
            int index = rnd.nextInt(3) + 1;
            intent.putExtra("message","Hello from NotifierTask " + counter);
            broadcastManager.sendBroadcast(intent);
            counter++;
            SystemClock.sleep(1000);

            /*if (isCancelled())
            {
                break;
            }*/
        }

        Log.d("TASK", "Cancelled!");

        return null;
    }
}
