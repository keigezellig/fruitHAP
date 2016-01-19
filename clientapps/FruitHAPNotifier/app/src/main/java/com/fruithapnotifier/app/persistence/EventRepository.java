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
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by maarten on 12/2/15.
 */
public class EventRepository
{


    private final LocalBroadcastManager broadcastManager;
    // Database fields
    private SqlHelper dbHelper;
    private String[] allColumns = { SqlHelper.COLUMN_ID, SqlHelper.COLUMN_EVENTDATA, SqlHelper.COLUMN_TYPE };
    private Context context;


    public EventRepository(Context context)
    {
        this.context = context;
        dbHelper = new SqlHelper(context);
        this.broadcastManager = LocalBroadcastManager.getInstance(context);
    }


    public Alert insertAlert(String alertData)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_EVENTDATA, alertData);
        values.put(SqlHelper.COLUMN_TYPE,EventType.ALERT.ordinal());

        long insertId = database.insert(SqlHelper.TABLE_EVENTS, null,
                values);

        Cursor cursor = database.query(SqlHelper.TABLE_EVENTS,
                allColumns,dbHelper.COLUMN_ID + "=?",new String[] {insertId + ""},null,null,null);

        Alert alert = null;
        cursor.moveToFirst();
        while (!cursor.isAfterLast())
        {
             alert = cursorToAlert(cursor);
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        dbHelper.close();

        Intent intent = new Intent(Constants.ALERT_INSERTED);

        intent.putExtra("ALERTDATA",alert);
        broadcastManager.sendBroadcast(intent);
        return alert;
    }

    public void deleteAlert(Alert alert)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        long id = alert.getId();
        Log.d("repos","Event deleted with id: " + id);
        database.delete(SqlHelper.TABLE_EVENTS, dbHelper.COLUMN_ID
                + " = " + id, null);
        dbHelper.close();
        Intent intent = new Intent(Constants.ALERT_DELETED);
        intent.putExtra("ALERTDATA",alert);
        broadcastManager.sendBroadcast(intent);
    }

    public List<Alert> getAllAlerts()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        List<Alert> alerts = new ArrayList<Alert>();

        Cursor cursor = database.query(SqlHelper.TABLE_EVENTS,
                allColumns, SqlHelper.COLUMN_TYPE + "=?", new String[] {EventType.ALERT.ordinal() + ""}, null, null, null);

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
        Cursor cursor = database.query(SqlHelper.TABLE_EVENTS,
                allColumns,dbHelper.COLUMN_ID + "=? AND "+SqlHelper.COLUMN_TYPE + "=?",new String[] {""+id, EventType.ALERT.ordinal() + ""},null,null,null);

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
            JSONObject eventData = new JSONObject(cursor.getString(1));
            Alert alert = new Alert(cursor.getInt(0), eventData);
            return alert;
        }
        catch (JSONException e)
        {
            Log.e("EventRepository", "Cannot retrieve alert:",e);
            return null;
        }

    }

    public void deleteAlerts()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        database.delete(SqlHelper.TABLE_EVENTS,SqlHelper.COLUMN_TYPE + "=?", new String[] {EventType.ALERT.ordinal() + ""});
        dbHelper.close();
        Intent intent = new Intent(Constants.ALERTS_CLEARED);
        broadcastManager.sendBroadcast(intent);
    }

}

