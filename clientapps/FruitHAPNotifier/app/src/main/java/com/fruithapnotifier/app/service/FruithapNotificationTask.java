package com.fruithapnotifier.app.service;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.widget.Toast;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.common.MessageCallback;
import com.fruithapnotifier.app.common.MqProvider;
import com.fruithapnotifier.app.mqproviders.MqProviderFactory;

/**
 * Created by developer on 12/5/15.
 */
public class FruithapNotificationTask extends AsyncTask<ConnectionParameters, Void, Void>
{
    private final MqProvider mqProvider;
    private final Context context;
    private String LOGTAG;
    private final Intent intent;
    private final LocalBroadcastManager broadcastManager;

    public FruithapNotificationTask(Context ctx)
    {
        this.broadcastManager = LocalBroadcastManager.getInstance(ctx);
        this.intent = new Intent(Constants.FRUITHAP_NOTIFICATION_ACTION);
        this.context = ctx;
        mqProvider = MqProviderFactory.getMqProviderInstance();
        LOGTAG = "FruithapNotificationTask";
    }



    @Override
    public Void doInBackground(ConnectionParameters... parameters)
    {
        try
        {
            initialize(parameters[0]);
            while (!isCancelled())
            {
                doWork();
            }

            Log.d(LOGTAG, "Cancelled!");

            cleanup();

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
            Toast.makeText(context, R.string.notification_service_error,Toast.LENGTH_SHORT).show();
            context.stopService(new Intent(context, FruithapPubSubService.class));
        }
        else
        {
            Toast.makeText(context, R.string.notification_service_stopped, Toast.LENGTH_SHORT).show();
        }
    }

    private void initialize(ConnectionParameters parameters) throws Exception
    {

        mqProvider.initialize(parameters.getHost(),parameters.getPort(),parameters.getUserName(),parameters.getPassword(),parameters.getvHost());
        mqProvider.setPubSubExchange(parameters.getPubSubExchange());
        mqProvider.subscribe(parameters.getPubSubTopics(), new MessageCallback()
        {
            @Override
            public void onMessageReceived(String topic, String message)
            {
                intent.putExtra(Constants.MQ_PUBSUB_TOPIC, topic);
                intent.putExtra(Constants.MQ_PUBSUB_MESSAGE, message);
                broadcastManager.sendBroadcast(intent);
            }
        });

    }

    private void doWork() throws Exception
    {
       mqProvider.processPublishedMessages();
    }

    protected void cleanup() throws Exception
    {
        mqProvider.close();
    }
}
