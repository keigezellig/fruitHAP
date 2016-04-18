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
import org.greenrobot.eventbus.EventBus;

public class SwitchViewModel extends SensorViewModel
{
    private Switch model;
    private SwitchViewState state;
    private String lastUpdated;


    public SwitchViewModel(String name, String description, String category, Switch model)
    {
        super(name, description, category);
        this.model = model;
    }

    @Override
    public int getViewType()
    {
        if (model.isReadOnly())
        {
            return VIEWTYPE_READONLYSWITCH;
        }
        else
        {
            return VIEWTYPE_SWITCH;
        }
    }

    public String getName()
    {
        return name;
    }

    public String getDescription()
    {
        return description;
    }

    public SwitchViewState getState()
    {
        return state;
    }

    public String getLastUpdated()
    {
        return lastUpdated;
    }

    public void setLastUpdated(String lastUpdated)
    {
        this.lastUpdated = lastUpdated;
    }

    public void setState(SwitchViewState newState, boolean shouldUpdateModel)
    {
        state = newState;
        if (shouldUpdateModel)
        {
            EventBus.getDefault().post(new SwitchViewStateChangeEvent(name, newState == SwitchViewState.ON));
        }
    }


}
