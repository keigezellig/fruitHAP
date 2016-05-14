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

public abstract class SensorViewModel
{

    public static final int VIEWTYPE_SWITCH = 0x01;
    public static final int VIEWTYPE_READONLYSWITCH = 0x02;
    public static final int VIEWTYPE_BUTTON = 0x03;
    public static final int VIEWTYPE_UNITVALUE = 0x04;
    public static final int VIEWTYPE_IMAGEVALUE = 0x05;
    public static final int VIEWTYPE_TEXTVALUE = 0x06;


    protected String name;
    protected String description;
    protected String category;

    public abstract int getViewType();

    public SensorViewModel(String name, String description, String category)
    {
        this.name = name;
        this.description = description;
        this.category = category;
    }

    public String getName()
    {
        return name;
    }

    public String getDescription()
    {
        return description;
    }

    public String getCategory()
    {
        return category;
    }
}
