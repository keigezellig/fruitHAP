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

package com.fruithapnotifier.app.service.configuration;

import android.util.Log;
import com.fruithapnotifier.app.common.ConfigurationEvent;
import com.fruithapnotifier.app.common.ConfigurationLoader;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.models.configuration.ConfigurationItem;
import com.fruithapnotifier.app.models.configuration.SensorType;
import com.fruithapnotifier.app.persistence.ConfigurationRepository;
import org.apache.commons.lang3.StringUtils;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class DatabaseConfigurationLoader implements ConfigurationLoader
{
    private static final String TAG = DatabaseConfigurationLoader.class.getName();
    private ConfigurationRepository repository;
    private RequestAdapter requestAdapter;

    public DatabaseConfigurationLoader(ConfigurationRepository repository, RequestAdapter requestAdapter)
    {
        this.repository = repository;
        this.requestAdapter = requestAdapter;
        EventBus.getDefault().register(this);
    }

    @Override
    public void loadConfiguration()
    {
        //TODO: For now clear database, in future more 'intelligent' way of loading config (e.g. with deltas)
        repository.deleteConfigurationItems();
        requestAdapter.sendConfigurationRequest("GetAllSensors",null);
    }

    @Subscribe
    public void onConfigurationReceived(ConfigurationEvent configurationEvent)
    {
        JSONObject eventData = configurationEvent.getEventData();
        try
        {
            if (eventData.getString("OperationName").equals("GetAllSensors"))
            {
                JSONObject dataObject = eventData.getJSONObject("Data");
                JSONArray sensorList = dataObject.getJSONArray("$values");

                for (int i = 0; i < sensorList.length(); i++)
                {
                    JSONObject sensorObject = sensorList.getJSONObject(i);
                    JSONObject parameters = sensorObject.getJSONObject("Parameters");
                    SensorType sensorType = determineSensorType(sensorObject.getString("Type"));
                    ConfigurationItem configItem = new ConfigurationItem(parameters.getString("Name"), parameters.getString("Description"), parameters.getString("Category"), sensorType);
                    repository.insertConfigurationItem(configItem);
                }
            }
            EventBus.getDefault().cancelEventDelivery(configurationEvent);
        }
        catch (JSONException jex)
        {
            Log.e(TAG,"Error loading configuration ",jex);
        }
     
    }

    private SensorType determineSensorType(String type)
    {
        //NB: This determine sensor type by convention, e.g. when 'Switch' is in the typename then type is Switch

        if (StringUtils.containsIgnoreCase(type,"Switch"))
        {
            return SensorType.Switch;
        }

        if (StringUtils.containsIgnoreCase(type,"Button"))
        {
            return SensorType.Button;
        }

        if (StringUtils.containsIgnoreCase(type,"Camera"))
        {
            return SensorType.ImageValue;
        }

        return SensorType.UnitValue;
    }
}
