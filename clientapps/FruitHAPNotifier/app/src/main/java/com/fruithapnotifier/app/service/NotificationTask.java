package com.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.SystemClock;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

/**
 * Created by developer on 11/28/15.
 */
public class NotificationTask extends AsyncTask<Void, Void, Void>
{
    private final Intent intent;
    private final LocalBroadcastManager broadcastManager;
    private static String INTENT_ACTION = "ACTION_FRUITHAP_NOTIFICATION";

    public NotificationTask(Context ctx) {
        this.broadcastManager = LocalBroadcastManager.getInstance(ctx);
        this.intent = new Intent(INTENT_ACTION);
    }

    @Override
    protected Void doInBackground(Void... voids)
    {
        int counter=0;
        while (!isCancelled())
        {
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
