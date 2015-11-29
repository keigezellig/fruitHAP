package com.fruithapnotifier.app;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import com.fruithapnotifier.app.service.EventNotificationService;

public class ServiceControlReceiver extends BroadcastReceiver
{
    private static String START_ACTION = "com.fruithapnotifier.app.action.START_SERVICE";
    private static String STOP_ACTION = "com.fruithapnotifier.app.action.STOP_SERVICE";

    public ServiceControlReceiver()
    {

    }

    @Override
    public void onReceive(Context context, Intent intent)
    {
        Intent serviceIntent = new Intent(context, EventNotificationService.class);

        String action = intent.getAction();
        Log.d("ServiceControlReceiver",action);
        if (action.equals(START_ACTION))
        {
            context.startService(serviceIntent);
        }
        else if (action.equals(STOP_ACTION))
        {
            context.stopService(serviceIntent);
        }

    }
}
