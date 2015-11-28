package com.example.fruithapnotifier.app.service;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;

public class NotifierService extends Service
{
    private NotifierTask notifierTask;
    public NotifierService()
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
        notifierTask = new NotifierTask(this);

    }

    @Override
    public void onDestroy()
    {
        notifierTask.cancel(true);
        super.onDestroy();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        notifierTask.execute(null);
        return START_STICKY;
    }
}
