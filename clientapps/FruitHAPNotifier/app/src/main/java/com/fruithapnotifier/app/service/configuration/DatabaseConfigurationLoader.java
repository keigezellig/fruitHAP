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

import android.app.Application;
import android.content.Context;
import android.util.Log;
import android.widget.Toast;
import com.fruithapnotifier.app.common.ConfigurationEvent;
import com.fruithapnotifier.app.common.ConfigurationLoader;
import com.fruithapnotifier.app.common.RequestAdapter;
import com.fruithapnotifier.app.common.RequestErrorEvent;
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
        requestAdapter.sendSensorConfigurationRequest("");
    }

    @Subscribe
    public void onConfigurationReceived(ConfigurationEvent configurationEvent)
    {
        repository.deleteConfigurationItems();
        JSONArray sensorConfiguration = configurationEvent.getEventData();
        try
        {
                for (int i = 0; i < sensorConfiguration.length(); i++)
                {
                    JSONObject sensorObject = sensorConfiguration.getJSONObject(i);
                    String name = sensorObject.getString("Name");
                    String description = sensorObject.getString("Description");
                    String category = sensorObject.getString("Category");
                    SensorType sensorType = determineSensorType(sensorObject);
                    if (sensorType != SensorType.NotSupported)
                    {
                        ConfigurationItem configItem = new ConfigurationItem(name, description,category, sensorType);
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

    @Subscribe
    public void onConfigurationError(RequestErrorEvent configurationErrorEvent)
    {
        repository.deleteConfigurationItems();
        EventBus.getDefault().cancelEventDelivery(configurationErrorEvent);

    }


    private SensorType determineSensorType(JSONObject sensorObject) throws JSONException
    {
        JSONObject supportedOperations = sensorObject.getJSONObject("SupportedOperations");
        String valueType = sensorObject.getString("ValueType");

        if (isButton(supportedOperations, valueType))
        {
            return SensorType.Button;
        }

        if (isSwitch(supportedOperations,valueType))
        {
            return SensorType.Switch;
        }

        if (isReadOnlySwitch(supportedOperations,valueType))
        {
            return SensorType.ReadOnlySwitch;
        }

        if (isImageValue(supportedOperations,valueType))
        {
            return SensorType.ImageValue;
        }

        if (isTextValue(supportedOperations,valueType))
        {
            return SensorType.TextValue;
        }

        if (isUnitValue(supportedOperations,valueType))
        {
            return SensorType.UnitValue;
        }

        return SensorType.NotSupported;
    }

    private boolean isUnitValue(JSONObject supportedOperations, String valueType)
    {
        return supportedOperations.has("GetValue") && supportedOperations.has("GetLastUpdateTime") && valueType.contains("QuantityValue");
    }

    private boolean isTextValue(JSONObject supportedOperations, String valueType)
    {
        return supportedOperations.has("GetValue") && supportedOperations.has("GetLastUpdateTime") && valueType.equals("TextValue");
    }

    private boolean isImageValue(JSONObject supportedOperations, String valueType)
    {
        return supportedOperations.has("GetValue") && supportedOperations.has("GetLastUpdateTime") && valueType.equals("ImageValue");
    }

    private boolean isSwitch(JSONObject supportedOperations, String valueType)
    {
        return supportedOperations.has("GetValue") && supportedOperations.has("GetLastUpdateTime") && valueType.equals("OnOffValue")
                && supportedOperations.has("TurnOn") && supportedOperations.has("TurnOff");
    }

    private boolean isReadOnlySwitch(JSONObject supportedOperations, String valueType)
    {
        return supportedOperations.has("GetValue") && supportedOperations.has("GetLastUpdateTime") && valueType.equals("OnOffValue");
    }

    private boolean isButton(JSONObject supportedOperations, String valueType)
    {
        return (supportedOperations.has("PressButton")) && (valueType == null);
    }
}
