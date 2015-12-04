package com.fruithapnotifier.app.persistence;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;
import com.fruithapnotifier.app.domain.SensorEvent;
import org.apache.commons.lang3.SerializationUtils;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

/**
 * Created by maarten on 12/2/15.
 */
public class SensorEventRepository
{
    // Database fields
    private SQLiteDatabase database;
    private SqlHelper dbHelper;
    private String[] allColumns = { SqlHelper.COLUMN_ID, SqlHelper.COLUMN_TIMESTAMP, SqlHelper.COLUMN_SENSORNAME, SqlHelper.COLUMN_OPTIONALDATA };

    public SensorEventRepository(Context context) {
        dbHelper = new SqlHelper(context);
    }

    public void open() throws SQLException {
        database = dbHelper.getWritableDatabase();
    }

    public void close() {
        dbHelper.close();
    }

    public SensorEvent createEvent(Long timestamp, String sensorName, Serializable optionalData)
    {
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_TIMESTAMP, timestamp);
        values.put(SqlHelper.COLUMN_SENSORNAME, sensorName);
        values.put(SqlHelper.COLUMN_OPTIONALDATA, SerializationUtils.serialize(optionalData));

        long insertId = database.insert(SqlHelper.TABLE_EVENTS, null,
                values);
        Cursor cursor = database.query(SqlHelper.TABLE_EVENTS,
                allColumns, SqlHelper.COLUMN_ID + " = " + insertId, null,
                null, null, null);
        cursor.moveToFirst();
        SensorEvent newEvent = cursorToSensorEvent(cursor);
        cursor.close();
        return newEvent;
    }

    public void deleteComment(SensorEvent event) {
        long id = event.getId();
        Log.d("repos","Event deleted with id: " + id);
        database.delete(SqlHelper.TABLE_EVENTS, dbHelper.COLUMN_ID
                + " = " + id, null);
    }

    public List<SensorEvent> getAllComments() {
        List<SensorEvent> events = new ArrayList<SensorEvent>();

        Cursor cursor = database.query(SqlHelper.TABLE_EVENTS,
                allColumns, null, null, null, null, null);

        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            SensorEvent event = cursorToSensorEvent(cursor);
            events.add(event);
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        return events;
    }

    private SensorEvent cursorToSensorEvent(Cursor cursor) {
        SensorEvent event = new SensorEvent(cursor.getInt(0), cursor.getString(1), new Date(cursor.getLong(2)), cursor.getBlob(3));
        return event;
    }
}

