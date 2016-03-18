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

package com.fruithapnotifier.app.ui.dashboard.viewmodels;

import com.fruithapnotifier.app.models.sensor.Switch;

public class SwitchViewModel
{
    private String name;
    private String description;
    private Switch model;
    private SwitchState state;
    private String lastUpdated;

    public SwitchViewModel(String name, String description, Switch model)
    {
        this.name = name;
        this.description = description;
        this.model = model;
    }

    public String getName()
    {
        return name;
    }

    public String getDescription()
    {
        return description;
    }

    public SwitchState getState()
    {
        return state;
    }

    public String getLastUpdated()
    {
        return lastUpdated;
    }

    public void setState(SwitchState newState, boolean shouldUpdateModel)
    {
        state = newState;
        if (shouldUpdateModel)
        {
            switch (newState)
            {
                case ON:
                    model.turnOn();
                    break;
                case OFF:
                    model.turnOff();
                    break;
            }
        }
    }

    public void setLastUpdated(String lastUpdated)
    {
        this.lastUpdated = lastUpdated;
    }
}
