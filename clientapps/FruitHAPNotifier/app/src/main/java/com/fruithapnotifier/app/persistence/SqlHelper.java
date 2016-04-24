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

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;


public class SqlHelper extends SQLiteOpenHelper {

    public static final String TABLE_ALERTS = "alerts";
    public static final String COLUMN_ALERT_ID = "_id";
    public static final String COLUMN_ALERT_TIMESTAMP = "timestamp";
    public static final String COLUMN_ALERT_SENSORNAME = "sensorName";
    public static final String COLUMN_ALERT_TEXT = "text";
    public static final String COLUMN_ALERT_PRIORITY = "priority";
    public static final String COLUMN_ALERT_OPTIONALDATA = "optionalData";
    public static final String COLUMN_ALERT_HASBEENREAD = "hasBeenRead";

    public static final String TABLE_CONFIG = "config";
    public static final String COLUMN_CONFIG_ID = "_id";
    public static final String COLUMN_CONFIG_SENSORNAME = "sensorName";
    public static final String COLUMN_CONFIG_DESCRIPTION = "description";
    public static final String COLUMN_CONFIG_CATEGORY = "category";
    public static final String COLUMN_CONFIG_TYPE = "type";
    
    private static final String DATABASE_NAME = "fruithap.db";
    private static final int DATABASE_VERSION = 7;

    // Database creation sql statement
    private static final String ALERT_TABLE = "create table "
            + TABLE_ALERTS + "(" + COLUMN_ALERT_ID
            + " integer primary key autoincrement, "
            + COLUMN_ALERT_TIMESTAMP
            + " text not null, "
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

    private static final String CONFIG_TABLE = "create table "
            + TABLE_CONFIG + "(" + COLUMN_CONFIG_ID
            + " integer primary key autoincrement, "
            + COLUMN_CONFIG_SENSORNAME
            + " text not null, "
            + COLUMN_CONFIG_DESCRIPTION
            + " text, "
            + COLUMN_CONFIG_CATEGORY
            + " text, "
            + COLUMN_CONFIG_TYPE
            + " integer "
            +");";

    public SqlHelper(Context context) 
    {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase database) 
    {
        database.execSQL(ALERT_TABLE);
        database.execSQL(CONFIG_TABLE);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) 
    {
        Log.w(SqlHelper.class.getName(),
                "Upgrading database from version " + oldVersion + " to "
                        + newVersion + ", which will destroy all old data");
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_ALERTS);
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_CONFIG);
        onCreate(db);
    }

}