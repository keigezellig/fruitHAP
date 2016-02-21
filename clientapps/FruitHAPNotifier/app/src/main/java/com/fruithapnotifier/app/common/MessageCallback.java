package com.fruithapnotifier.app.common;

import org.json.JSONObject;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public interface MessageCallback
{
    void onMessageReceived(String topic, String message);
}
