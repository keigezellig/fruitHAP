package com.fruithapnotifier.app.service;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;
import android.widget.Toast;

public class NotificationService extends Service
{
    private NotificationTask notificationTask;
    public NotificationService()
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
        notificationTask = new NotificationTask(this);
        Toast.makeText(this, "My Service created", Toast.LENGTH_LONG).show();

    }

    @Override
    public void onDestroy()
    {
        Toast.makeText(this, "My Service stopped", Toast.LENGTH_LONG).show();
        notificationTask.cancel(true);
        super.onDestroy();
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId)
    {
        Toast.makeText(this, "My Service Started", Toast.LENGTH_LONG).show();
        notificationTask.execute((Void)null);
        return START_NOT_STICKY;
    }

}
