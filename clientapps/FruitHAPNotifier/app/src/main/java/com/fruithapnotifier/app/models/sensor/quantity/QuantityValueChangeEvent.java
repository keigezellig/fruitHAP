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

package com.fruithapnotifier.app.models.sensor.quantity;

import com.fruithapnotifier.app.models.sensor.Sensor;
import org.joda.time.DateTime;

/**
 * Created by MJOX03 on 18.4.2016.
 */
public class QuantityValueChangeEvent
{
    private final Sensor sender;
    private final QuantityValue value;
    private final DateTime date;

    public QuantityValueChangeEvent(Sensor sender, QuantityValue value, DateTime date)
    {
        this.sender = sender;
        this.value = value;
        this.date = date;
    }

    public Sensor getSender()
    {
        return sender;
    }

    public QuantityValue getValue()
    {
        return value;
    }

    public DateTime getDate()
    {
        return date;
    }
}
