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

package com.fruithapnotifier.app.service.requestadapter.requests;

import org.apache.commons.lang3.NotImplementedException;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

public class SensorRequest
{
    private String sensorName;
    private String operationName;
    private Map<String, String> parameters;

    public SensorRequest(String sensorName, String operationName, Map<String, String> parameters)
    {
        this.sensorName = sensorName;
        this.operationName = operationName;
        this.parameters = parameters;
    }

    public JSONObject toJson() throws JSONException
    {
        if (this.operationName.equals("GetValue"))
        {
            return createGetValueRequest();
        }

        return createCommandRequest();

    }

    private JSONObject createCommandRequest() throws JSONException
    {
        Map<String, String> requestObject = new HashMap<>();
        requestObject.put("SensorName",sensorName);
        requestObject.put("OperationName",operationName);
        JSONObject result = new JSONObject(requestObject);
        if (parameters != null)
        {
            JSONObject params = new JSONObject(this.parameters);
            result.put("Parameters",params);
        }
        return result;
    }

    private JSONObject createGetValueRequest()
    {
        Map<String, String> requestObject = new HashMap<>();
        requestObject.put("SensorName",sensorName);
        requestObject.put("EventType",operationName);
        return new JSONObject(requestObject);
    }
}
