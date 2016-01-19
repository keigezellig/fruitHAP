package com.fruithapnotifier.app.persistence;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.domain.Event;
import com.fruithapnotifier.app.domain.SensorEvent;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by maarten on 12/2/15.
 */
public class EventRepository
{


    // Database fields
    private SqlHelper dbHelper;
    private String[] allColumns = { SqlHelper.COLUMN_ID, SqlHelper.COLUMN_EVENTDATA, SqlHelper.COLUMN_TYPE };
    private Context context;


    public EventRepository(Context context)
    {
        this.context = context;
        dbHelper = new SqlHelper(context);
    }


    public long insertEvent(String eventData, EventType type)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_EVENTDATA, eventData);
        values.put(SqlHelper.COLUMN_TYPE,type.ordinal());

        long insertId = database.insert(SqlHelper.TABLE_EVENTS, null,
                values);
        dbHelper.close();
        return insertId;
    }

    public void deleteEvent(Event event)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        long id = event.getId();
        Log.d("repos","Event deleted with id: " + id);
        database.delete(SqlHelper.TABLE_EVENTS, dbHelper.COLUMN_ID
                + " = " + id, null);
        dbHelper.close();
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

    public void deleteAll(EventType type)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        database.delete(SqlHelper.TABLE_EVENTS,SqlHelper.COLUMN_TYPE + "=?", new String[] {type.ordinal() + ""});
        dbHelper.close();
    }

}

