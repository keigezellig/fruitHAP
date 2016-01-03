package com.fruithapnotifier.app.ui;

import android.app.NotificationManager;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.GradientDrawable;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Base64;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;


import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.common.Constants;
import com.fruithapnotifier.app.domain.SensorEvent;
import com.fruithapnotifier.app.persistence.SensorEventRepository;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.joda.time.format.DateTimeFormat;

/**
 * A fragment representing a single EventNotification detail screen.
 * This fragment is either contained in a {@link EventNotificationListActivity}
 * in two-pane mode (on tablets) or a {@link EventNotificationDetailActivity}
 * on handsets.
 */
public class EventNotificationDetailFragment extends Fragment {
    /**
     * The fragment argument representing the item ID that this fragment
     * represents.
     */
    public static final String ARG_ITEM_ID = "item_id";

    /**
     * The dummy content this fragment is presenting.
     */
    private SensorEvent mItem;
    private SensorEventRepository datasource;

    /**
     * Mandatory empty constructor for the fragment manager to instantiate the
     * fragment (e.g. upon screen orientation changes).
     */
    public EventNotificationDetailFragment() {
    }

    @Override
    public void onAttach(Context context)
    {
        super.onAttach(context);
        datasource = new SensorEventRepository(context);
        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        notificationManager.cancel(Constants.INCOMING_EVENT_NOTIFICATIONID);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (getArguments().containsKey(ARG_ITEM_ID))
        {
            datasource.open();
            mItem = datasource.getEventById(getArguments().getInt(ARG_ITEM_ID));
            datasource.close();
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_eventnotification_detail, container, false);


        try
        {
            // Show the dummy content as text in a TextView.
            if (mItem != null)
            {
                TextView txtPriority = (TextView) rootView.findViewById(R.id.txtPriority);
                GradientDrawable background = (GradientDrawable) txtPriority.getBackground();
                background.setColor(PriorityHelpers.ConvertToColor(mItem.getNotificationPriority()));
                txtPriority.setText(PriorityHelpers.GetTextResource(mItem.getNotificationPriority()));

                TextView txtTimestamp = (TextView) rootView.findViewById(R.id.txtTimestamp);
                txtTimestamp.setText(DateTimeFormat.forStyle("SL").print(mItem.getTimestamp()));

                TextView txtSensorName = (TextView) rootView.findViewById(R.id.txtSensorName);
                txtSensorName.setText(mItem.getSensorName());

                TextView txtMessage = (TextView) rootView.findViewById(R.id.txtMessage);
                txtMessage.setText(mItem.getNotificationText());

                if ( (!mItem.getOptionalData().isNull("$type")) && (mItem.getOptionalData().getString("$type").contains("Byte")))
                {
                    String imageString = mItem.getOptionalData().getString("$value");
                    byte[] decodedString = Base64.decode(imageString, Base64.DEFAULT);
                    Bitmap decodedImage = BitmapFactory.decodeByteArray(decodedString, 0, decodedString.length);
                    ImageView imgOptional = (ImageView) rootView.findViewById(R.id.imgOptional);
                    imgOptional.setImageBitmap(decodedImage);
                }
            }
        }
        catch (Exception e)
        {
            Log.e(this.getClass().getName(),"Error rendering view: ",e);
        }

        return rootView;
    }
}
