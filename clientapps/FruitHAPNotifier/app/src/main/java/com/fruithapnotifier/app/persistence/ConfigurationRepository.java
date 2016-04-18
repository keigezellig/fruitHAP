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
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import com.fruithapnotifier.app.models.configuration.ConfigurationItem;
import com.fruithapnotifier.app.models.configuration.SensorType;
import com.fruithapnotifier.app.models.sensor.*;
import com.fruithapnotifier.app.models.sensor.button.Button;
import com.fruithapnotifier.app.models.sensor.image.ImageSensor;
import com.fruithapnotifier.app.models.sensor.quantity.QuantitySensor;
import com.fruithapnotifier.app.models.sensor.switchy.Switch;
import com.fruithapnotifier.app.models.sensor.text.TextSensor;

import java.util.ArrayList;
import java.util.List;

public class ConfigurationRepository
{
    private final Context context;
    private SqlHelper dbHelper;
    private String[] allColumns =
            {
                    SqlHelper.COLUMN_CONFIG_ID,
                    SqlHelper.COLUMN_CONFIG_SENSORNAME,
                    SqlHelper.COLUMN_CONFIG_DESCRIPTION,
                    SqlHelper.COLUMN_CONFIG_CATEGORY,
                    SqlHelper.COLUMN_CONFIG_TYPE,
            };


    public ConfigurationRepository(Context context)
    {
        this.context = context;
        dbHelper = new SqlHelper(context);
    }

    public void insertConfigurationItem (ConfigurationItem cfgItem)
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        ContentValues values = new ContentValues();
        values.put(SqlHelper.COLUMN_CONFIG_SENSORNAME, cfgItem.getSensorName());
        values.put(SqlHelper.COLUMN_CONFIG_DESCRIPTION, cfgItem.getDescription());
        values.put(SqlHelper.COLUMN_CONFIG_CATEGORY, cfgItem.getCategory());
        values.put(SqlHelper.COLUMN_CONFIG_TYPE, cfgItem.getSensorType().ordinal());
        database.insert(SqlHelper.TABLE_CONFIG, null, values);
        dbHelper.close();
    }

    public void deleteConfigurationItems()
    {
        SQLiteDatabase database = dbHelper.getWritableDatabase();
        database.delete(SqlHelper.TABLE_CONFIG,null, null);
        dbHelper.close();
    }

    public List<Sensor> getSensors()
    {
        List<Sensor> sensors = new ArrayList<>();
        SQLiteDatabase database = dbHelper.getWritableDatabase();

        Cursor cursor = database.query(SqlHelper.TABLE_CONFIG, allColumns, null, null, null, null,null);

        cursor.moveToFirst();
        while (!cursor.isAfterLast())
        {
            Sensor sensor = cursorToSensor(cursor);
            if (sensor != null)
            {
                sensors.add(sensor);
            }
            cursor.moveToNext();
        }
        // make sure to close the cursor
        cursor.close();
        dbHelper.close();
        return sensors;

    }

    private Sensor cursorToSensor(Cursor cursor)
    {
        String sensorName = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_CONFIG_SENSORNAME));
        String description = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_CONFIG_DESCRIPTION));
        String category = cursor.getString(cursor.getColumnIndex(SqlHelper.COLUMN_CONFIG_CATEGORY));
        SensorType sensorType = SensorType.values()[cursor.getInt(cursor.getColumnIndex(SqlHelper.COLUMN_CONFIG_TYPE))];
        switch (sensorType)
        {
            case Switch:
                return new Switch(sensorName,description,category,false, context);
            case ReadOnlySwitch:
                return new Switch(sensorName,description,category,true, context);
            case Button:
                return new Button(sensorName,description,category, context);
            case UnitValue:
                return new QuantitySensor(sensorName,description,category,true,context);
            case ImageValue:
                return new ImageSensor(sensorName,description,category,true,context);
            case TextValue:
                return new TextSensor(sensorName,description,category,true,context);
            default:
                return null;

        }

    }


}
