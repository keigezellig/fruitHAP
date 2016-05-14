
/*
 *  -----------------------------------------------------------------------------------------------
 *  "THE APPRECIATION LICENSE" (Revision 0x100):
 *  Copyright (c) 2016. M. Joosten
 *  You can do anything with this program and its code, even use it to run a nuclear reactor (why should you)
 *  But I won't be responsible for possible damage and mishap, because i never tested it on a nuclear reactor (why should I..)
 *  If you think this program/code is absolutely great and supercalifragilisticexpialidocious (or just plain useful), just
 *  let me know by sending me a nice email or postcard from your country of origin and leave this header in the code.
 *  Or better join my efforts to make this program even better :-)
 *  See my blog (http://joosten-industries/blog), for contact info
 *  ---------------------------------------------------------------------------------------------------
 *
 *
 */

package com.fruithapnotifier.app.persistence;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.models.alert.Alert;
import com.fruithapnotifier.app.models.alert.AlertPriority;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;


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
                    SqlHelper.COLUMN_ALERT_OPTIONALDATA,
                    SqlHelper.COLUMN_ALERT_HASBEENREAD
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
        DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
        values.put(SqlHelper.COLUMN_ALERT_TIMESTAMP, alert.getTimestamp().toString(fmt));
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

        intent.putExtra(Constants.ALERT_DATA,insertedAlert);
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
        intent.putExtra(Constants.ALERT_DATA,alert);
        broadcastManager.sendBroadcast(intent);
        Log.d("repos","Event deleted with id: " + id);
    }

    public void updateAlert(Alert alert)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        long id = alert.getId();
        ContentValues values = new ContentValues();
        DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
        values.put(SqlHelper.COLUMN_ALERT_TIMESTAMP, alert.getTimestamp().toString(fmt));
        values.put(SqlHelper.COLUMN_ALERT_SENSORNAME,alert.getSensorName());
        values.put(SqlHelper.COLUMN_ALERT_TEXT,alert.getNotificationText());
        values.put(SqlHelper.COLUMN_ALERT_PRIORITY,alert.getNotificationPriority().ordinal());
        values.put(SqlHelper.COLUMN_ALERT_OPTIONALDATA,alert.getOptionalData().toString());
        values.put(SqlHelper.COLUMN_ALERT_HASBEENREAD,alert.isRead());
        int r = database.update(SqlHelper.TABLE_ALERTS,values,dbHelper.COLUMN_ALERT_ID + "=?",new String[] {""+id});
        Log.d("repos","Updated records: " + r);

        Intent intent = new Intent(Constants.ALERT_UPDATED);
        intent.putExtra(Constants.ALERT_DATA,alert);
        broadcastManager.sendBroadcast(intent);
        Log.d("repos","Event updated. id: " + id);
        dbHelper.close();

    }

    public void markAllAsRead()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_ALERT_HASBEENREAD,true);
        int r = database.update(SqlHelper.TABLE_ALERTS,values,SqlHelper.COLUMN_ALERT_HASBEENREAD + "=?",new String[] {""+0});
        Log.d("repos","Updated records: " + r);

        ArrayList<Alert> updated = new ArrayList(getAllAlerts());
        Intent intent = new Intent(Constants.ALERTS_RANGEUPDATED);
        intent.putParcelableArrayListExtra(Constants.ALERT_RANGEDATA,updated);
        broadcastManager.sendBroadcast(intent);
        Log.i("repos","Marked all unread alerts as read");
        dbHelper.close();
    }

    public List<Alert> getAllAlerts()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        List<Alert> alerts = new ArrayList<Alert>();

        Cursor cursor = database.query(SqlHelper.TABLE_ALERTS, allColumns, null, null, null, null, SqlHelper.COLUMN_ALERT_ID + " DESC");

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
            DateTimeFormatter fmt = ISODateTimeFormat.dateTime();
            DateTime timestamp = new DateTime(fmt.parseDateTime(cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_TIMESTAMP))));
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

            boolean hasBeenRead = cursor.getInt(cursor.getColumnIndex(SqlHelper.COLUMN_ALERT_HASBEENREAD)) != 0;

            Alert alert = new Alert(id,timestamp,sensorName,text,prio,optionalData,hasBeenRead);
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

