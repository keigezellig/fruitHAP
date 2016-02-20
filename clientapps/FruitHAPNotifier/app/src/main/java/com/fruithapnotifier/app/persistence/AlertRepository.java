package com.fruithapnotifier.app.persistence;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.domain.AlertPriority;
import org.joda.time.DateTime;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by maarten on 12/2/15.
 */
public class AlertRepository
{

    private final LocalBroadcastManager broadcastManager;
    private SqlHelper dbHelper;
    private String[] allColumns =
            {
                    SqlHelper.COLUMN_ALERT_ID,
                    SqlHelper.COLUMN_ALERT_TIMESTAMP,
                    SqlHelper.COLUMN_ALERT_SENSORNAME,
                    SqlHelper.COLUMN_ALERT_TEXT,
                    SqlHelper.COLUMN_ALERT_PRIORITY,
                    SqlHelper.COLUMN_ALERT_OPTIONALDATA
            };

    private Context context;


    public AlertRepository(Context context)
    {
        this.context = context;
        dbHelper = new SqlHelper(context);
        this.broadcastManager = LocalBroadcastManager.getInstance(context);
    }


    public void insertAlert(Alert alert)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_ALERT_TIMESTAMP, alert.getTimestamp().getMillis());
        values.put(SqlHelper.COLUMN_ALERT_SENSORNAME,alert.getSensorName());
        values.put(SqlHelper.COLUMN_ALERT_TEXT,alert.getNotificationText());
        values.put(SqlHelper.COLUMN_ALERT_PRIORITY,alert.getNotificationPriority().ordinal());
        if (alert.getOptionalData() != null)
        {
            values.put(SqlHelper.COLUMN_ALERT_OPTIONALDATA, alert.getOptionalData().toString());
        }
        values.put(SqlHelper.COLUMN_ALERT_HASBEENREAD,alert.isRead());


        long insertId = database.insert(SqlHelper.TABLE_ALERTS, null,
                values);

        Cursor cursor = database.query(SqlHelper.TABLE_ALERTS,
                allColumns,dbHelper.COLUMN_ALERT_ID + "=?",new String[] {insertId + ""},null,null,null);

        Alert insertedAlert = null;
        cursor.moveToFirst();
        while (!cursor.isAfterLast())
        {
            insertedAlert = cursorToAlert(cursor);
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        dbHelper.close();

        Intent intent = new Intent(Constants.ALERT_INSERTED);

        intent.putExtra("ALERTDATA",insertedAlert);
        broadcastManager.sendBroadcast(intent);
    }

    public void deleteAlert(Alert alert)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        long id = alert.getId();
        database.delete(SqlHelper.TABLE_ALERTS, dbHelper.COLUMN_ALERT_ID
                + " = " + id, null);
        dbHelper.close();
        Intent intent = new Intent(Constants.ALERT_DELETED);
        intent.putExtra("ALERTDATA",alert);
        broadcastManager.sendBroadcast(intent);
        Log.d("repos","Event deleted with id: " + id);
    }

    public void updateAlert(Alert alert)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        long id = alert.getId();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_ALERT_TIMESTAMP, alert.getTimestamp().getMillis());
        values.put(SqlHelper.COLUMN_ALERT_SENSORNAME,alert.getSensorName());
        values.put(SqlHelper.COLUMN_ALERT_TEXT,alert.getNotificationText());
        values.put(SqlHelper.COLUMN_ALERT_PRIORITY,alert.getNotificationPriority().ordinal());
        values.put(SqlHelper.COLUMN_ALERT_OPTIONALDATA,alert.getOptionalData().toString());
        values.put(SqlHelper.COLUMN_ALERT_HASBEENREAD,alert.isRead());
        database.update(SqlHelper.TABLE_ALERTS,values,dbHelper.COLUMN_ALERT_ID + "=?",new String[] {""+id});

        Intent intent = new Intent(Constants.ALERT_UPDATED);
        intent.putExtra("ALERTDATA",alert);
        broadcastManager.sendBroadcast(intent);
        Log.d("repos","Event updated. id: " + id);

    }

    public List<Alert> getAllAlerts()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        List<Alert> alerts = new ArrayList<Alert>();

        Cursor cursor = database.query(SqlHelper.TABLE_ALERTS, allColumns, null, null, null, null, SqlHelper.COLUMN_ALERT_TIMESTAMP + " DESC");

        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            Alert alert = cursorToAlert(cursor);
            alerts.add(alert);
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        dbHelper.close();
        return alerts;
    }

    public Alert getAlertById(int id)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        Alert alert = null;
        Cursor cursor = database.query(SqlHelper.TABLE_ALERTS,
                allColumns,dbHelper.COLUMN_ALERT_ID + "=?",new String[] {""+id},null,null,null);

        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            alert = cursorToAlert(cursor);
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        dbHelper.close();
        return alert;
    }

    private Alert cursorToAlert(Cursor cursor)
    {
        try
        {
            int id = cursor.getInt(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_ID));
            DateTime timestamp = new DateTime(cursor.getInt(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_TIMESTAMP)));
            String sensorName = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_SENSORNAME));
            String text = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_TEXT));
            AlertPriority prio = AlertPriority.values()[cursor.getInt(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_PRIORITY))];
            String optionalStuff = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_OPTIONALDATA));
            JSONObject optionalData = new JSONObject();
            if (optionalStuff != null && (!optionalStuff.isEmpty()))
            {
                try
                {
                    optionalData = new JSONObject(optionalStuff);
                }
                catch (JSONException jex)
                {
                    Log.w("AlertRepository", "Invalid format of optional data, will be ignored",jex);
                }
            }

            Alert alert = new Alert(id,timestamp,sensorName,text,prio,optionalData);
            return alert;
        }
        catch (Exception e)
        {
            Log.e("AlertRepository", "Cannot retrieve alert:",e);
            return null;
        }
    }

    public void deleteAlerts()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        database.delete(SqlHelper.TABLE_ALERTS,null, null);
        dbHelper.close();
        Intent intent = new Intent(Constants.ALERTS_CLEARED);
        broadcastManager.sendBroadcast(intent);
    }

}

