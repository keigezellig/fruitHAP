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

import android.net.Uri;

public class SensorRequest
{
    private static final String BASEPATH = "/api/sensor/";
    private String sensorName;
    private String operationName;

    public SensorRequest(String sensorName, String operationName)
    {
        this.sensorName = sensorName;
        this.operationName = operationName;

    }

    public Uri getUri(Uri baseUri)
    {
        //GET /api/sensor/{name}/{operation}
        Uri.Builder uriBuilder = baseUri.buildUpon();
        uriBuilder.appendPath(BASEPATH).appendPath(sensorName);
        if (operationName != null || !operationName.isEmpty())
        {
            uriBuilder.appendPath(operationName);
        }


        return uriBuilder.build();

    }
}
