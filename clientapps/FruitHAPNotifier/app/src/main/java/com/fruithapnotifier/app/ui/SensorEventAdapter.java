package com.fruithapnotifier.app.ui;

import android.content.Context;
import android.graphics.drawable.GradientDrawable;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.SensorEvent;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by developer on 1/3/16.
 */
public class SensorEventAdapter extends ArrayAdapter<SensorEvent>
{
    private final List<SensorEvent> events;

    public SensorEventAdapter(Context context, int textViewResourceId, List<SensorEvent> events)
    {
        super(context, textViewResourceId, events);
        this.events = events;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent)
    {
        View v = convertView;
        if (v == null)
        {
            LayoutInflater vi = (LayoutInflater)getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            v = vi.inflate(R.layout.notification_listitem, null);
        }

        SensorEvent event = events.get(position);
        if (event != null)
        {
            try
            {
                TextView txtPriority = (TextView) v.findViewById(R.id.txtPriority);
                GradientDrawable background = (GradientDrawable) txtPriority.getBackground();
                background.setColor(PriorityHelpers.ConvertToColor(event.getNotificationPriority()));
                txtPriority.setText(PriorityHelpers.GetTextResource(event.getNotificationPriority()));

                TextView txtTimestamp = (TextView) v.findViewById(R.id.txtTimestamp);
                txtTimestamp.setText(DateTimeFormat.forStyle("SL").print(event.getTimestamp()));

                TextView txtMessage = (TextView) v.findViewById(R.id.txtMessage);
                txtMessage.setText(event.getNotificationText());
            }
            catch (JSONException e)
            {
                Log.e(this.getClass().getName(),"Error:",e);
            }
        }
        return v;

    }
}
