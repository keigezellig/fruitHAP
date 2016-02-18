package com.fruithapnotifier.app.persistence;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

/**
 * Created by maarten on 12/2/15.
 */
public class SqlHelper extends SQLiteOpenHelper {

    public static final String TABLE_ALERTS = "alerts";
    public static final String COLUMN_ALERT_ID = "_id";
    public static final String COLUMN_ALERT_TIMESTAMP = "timestamp";
    public static final String COLUMN_ALERT_SENSORNAME = "sensorName";
    public static final String COLUMN_ALERT_TEXT = "text";
    public static final String COLUMN_ALERT_PRIORITY = "priority";
    public static final String COLUMN_ALERT_OPTIONALDATA = "optionalData";
    public static final String COLUMN_ALERT_HASBEENREAD = "hasBeenRead";
    private static final String DATABASE_NAME = "fruithap.db";
    private static final int DATABASE_VERSION = 4;



    // Database creation sql statement
    private static final String DATABASE_CREATE = "create table "
            + TABLE_ALERTS + "(" + COLUMN_ALERT_ID
            + " integer primary key autoincrement, "
            + COLUMN_ALERT_TIMESTAMP
            + " integer not null, "
            + COLUMN_ALERT_SENSORNAME
            + " text not null, "
            + COLUMN_ALERT_TEXT
            + " text null, "
            + COLUMN_ALERT_PRIORITY
            + " integer, "
            + COLUMN_ALERT_OPTIONALDATA
            + " text, "
            + COLUMN_ALERT_HASBEENREAD
            + " int "
            +");";

    public SqlHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase database) {
        database.execSQL(DATABASE_CREATE);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        Log.w(SqlHelper.class.getName(),
                "Upgrading database from version " + oldVersion + " to "
                        + newVersion + ", which will destroy all old data");
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_ALERTS);
        onCreate(db);
    }

}