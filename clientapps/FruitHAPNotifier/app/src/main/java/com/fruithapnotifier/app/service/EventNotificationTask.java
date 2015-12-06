package com.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.widget.Toast;
import com.fruithapnotifier.app.common.Constants;

/**
 * Created by developer on 12/5/15.
 */
public abstract class EventNotificationTask extends AsyncTask<String, Void, Void>
{
    private final Context context;
    protected String LOGTAG;
    protected final Intent intent;
    protected final LocalBroadcastManager broadcastManager;

    public EventNotificationTask(Context ctx)
    {
        this.broadcastManager = LocalBroadcastManager.getInstance(ctx);
        this.intent = new Intent(Constants.SENSOREVENT_ACTION);
        this.context = ctx;
    }



    @Override
    public Void doInBackground(String... parameters)
    {
        try
        {
            initialize(parameters);
            while (!isCancelled())
            {
                doWork(parameters);
            }

            Log.d(LOGTAG, "Cancelled!");

            cleanup(parameters);

        }
        catch (Exception ex)
        {
            Log.e(LOGTAG,"Oops. Something went wrong:",ex);
        }

        return null;

    }

    @Override
    protected void onPostExecute(Void aVoid)
    {
        if (!isCancelled()) //Terminated in an error so stop the service
        {
            Toast.makeText(context,"Something went wrong while executing the service and will be stopped now",Toast.LENGTH_SHORT).show();
            context.stopService(new Intent(context, EventNotificationService.class));
        }
    }

    protected abstract void initialize(String[] parameters) throws Exception;
    protected abstract void doWork(String[] parameters) throws Exception;
    protected abstract void cleanup(String[] parameters) throws Exception;
}
