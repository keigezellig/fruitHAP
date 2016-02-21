package com.myown.rabbitmqtest.common;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public interface IMQSubscriptionReceiver
{
    void onMessageReceived(String topic, String message);
}
