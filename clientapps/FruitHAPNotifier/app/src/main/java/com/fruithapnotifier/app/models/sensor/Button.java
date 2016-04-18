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

package com.fruithapnotifier.app.models.sensor;

import android.content.Context;
import com.fruithapnotifier.app.service.requestadapter.RestRequestAdapter;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.ButtonPressFromViewEvent;
import org.greenrobot.eventbus.EventBus;
import org.greenrobot.eventbus.Subscribe;

public class Button implements Sensor
{
    private final RestRequestAdapter requestAdapter;
    private String name;
    private String description;
    private String category;

    @Override
    public String getName()
    {
        return name;
    }

    @Override
    public String getDescription()
    {
        return description;
    }

    @Override
    public String getCategory()
    {
        return category;
    }


    public Button(String name, String description, String category, Context ctx)
    {
        this.name = name;
        this.description = description;
        this.category = category;

        requestAdapter = new RestRequestAdapter(ctx);
    }

    private void pressButton()
    {
        requestAdapter.sendSensorRequest(name,"PressButton");
    }

    @Subscribe
    public void onButtonPressFromViewReceived(ButtonPressFromViewEvent viewPressEvent)
    {
        pressButton();
    }

    @Override
    public void registerForEvents()
    {
        EventBus.getDefault().register(this);
    }

    @Override
    public void unregisterForEvents()
    {
        EventBus.getDefault().unregister(this);
    }
}
