package com.fruithapnotifier.app.mqproviders;

import com.fruithapnotifier.app.common.MqProvider;

/**
 * Created by developer on 12/12/15.
 */
public class MqProviderFactory
{
    private static MqProvider instance;

    public static MqProvider getMqProviderInstance()
    {
        if (instance == null)
        {
            instance = new RabbitMqProvider();
        }
        return instance;
    }
}
