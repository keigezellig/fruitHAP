package com.fruithapnotifier.app.models.sensor;

import org.joda.time.DateTime;

import java.util.Date;


public class SwitchChangeEvent
{
    private final Switch sender;
    private final SwitchState switchState;
    private final DateTime date;

    public SwitchChangeEvent(Switch sender, SwitchState newValue, DateTime date)
    {
        this.sender = sender;
        this.date = date;
        this.switchState = newValue;
    }

    public SwitchState getSwitchState()
    {
        return switchState;
    }

    public Switch getSender()
    {
        return sender;
    }

    public DateTime getDate()
    {
        return date;
    }
}
