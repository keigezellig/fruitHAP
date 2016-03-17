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

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class ConfigurationRequest
{
    private static final int MESSAGETYPE_REQUEST = 0;
    private String operationName;
    private Map<String, String> parameters;

    public ConfigurationRequest(String operationName, Map<String, String> parameters)
    {
        this.operationName = operationName;
        this.parameters = parameters;
    }

    public JSONObject toJson() throws JSONException
    {
        JSONObject requestObject = new JSONObject();
        requestObject.put("OperationName",operationName);
        requestObject.put("MessageType",MESSAGETYPE_REQUEST);
        JSONObject params = new JSONObject();
        if (parameters != null)
        {
            params = new JSONObject(this.parameters);
        }
        requestObject.put("Parameters",params);
        return requestObject;
    }

}
