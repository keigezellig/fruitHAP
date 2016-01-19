package com.fruithapnotifier.app.persistence;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

/**
 * Created by maarten on 12/2/15.
 */
public class SqlHelper extends SQLiteOpenHelper {

    public static final String TABLE_EVENTS = "events";
    public static final String COLUMN_ID = "_id";
    public static final String COLUMN_EVENTDATA = "eventData";
    public static final String COLUMN_TYPE = "type";
    private static final String DATABASE_NAME = "fruithap.db";
    private static final int DATABASE_VERSION = 3;



    // Database creation sql statement
    private static final String DATABASE_CREATE = "create table "
            + TABLE_EVENTS + "(" + COLUMN_ID
            + " integer primary key autoincrement, "
            + COLUMN_EVENTDATA
            + " text not null, "
            + COLUMN_TYPE
            + " integer "

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
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_EVENTS);
        onCreate(db);
    }

}