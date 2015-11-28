package com.example.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v4.content.LocalBroadcastManager;

/**
 * Created by maarten on 11/27/15.
 */
public class NotifierTask extends AsyncTask<NotifierTaskParameters, Void, Void>
{
    private final Intent intent;
    private final LocalBroadcastManager broadcastManager;
    private static String INTENT_ACTION = "ACTION_FRUITHAP_NOTIFICATION";

    public NotifierTask(Context ctx) {
        this.broadcastManager = LocalBroadcastManager.getInstance(ctx);
        this.intent = new Intent(INTENT_ACTION);
    }

    @Override
    protected Void doInBackground(NotifierTaskParameters... notifierTaskParameters)
    {
        int counter=0;
        while (!isCancelled())
        {
            intent.putExtra("message","Hello from NotifierTask " + counter);
            broadcastManager.sendBroadcast(intent);
            counter++;
        }

        return null;


    }
}
