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

package com.fruithapnotifier.app.ui.dashboard.viewmodels.quantity;

import com.fruithapnotifier.app.ui.dashboard.viewmodels.SensorViewModel;

/**
 * Created by MJOX03 on 19.4.2016.
 */
public class QuantityViewModel extends SensorViewModel
{
   private double value;
    private String unitText;
    private String lastUpdated;

    public QuantityViewModel(String name, String description, String category)
    {
        super(name, description, category);

    }

    @Override
    public int getViewType() {
        return VIEWTYPE_UNITVALUE;
    }

    public double getValue()
    {
        return value;
    }

    public void setValue(double value) {
        this.value = value;
    }

    public String getUnitText() {
        return unitText;
    }

    public void setUnitText(String unitText) {
        this.unitText = unitText;
    }

    public String getLastUpdated()
    {
        return lastUpdated;
    }

    public void setLastUpdated(String lastUpdated)
    {
        this.lastUpdated = lastUpdated;
    }
}
