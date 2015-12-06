package com.fruithapnotifier.app.service;

import android.content.Context;
import android.os.SystemClock;

import java.util.Date;

/**
 * Created by developer on 11/28/15.
 */
public class TestNotificationTask extends EventNotificationTask
{
    public TestNotificationTask(Context ctx)
    {
        super(ctx);
        LOGTAG = "TestNotificationTask";
    }

    @Override
    protected void cleanup(String[] parameters)
    {

    }

    @Override
    protected void initialize(String[] parameters)
    {

    }

    @Override
    protected void doWork(String[] parameters)
    {
        intent.putExtra("timestamp", new Date().getTime());
        intent.putExtra("sensorName", "Doorbell");
        intent.putExtra("optionalData", (byte[]) null);

        broadcastManager.sendBroadcast(intent);
        SystemClock.sleep(5000);
    }
}
