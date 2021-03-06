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

package com.fruithapnotifier.app.ui.dashboard;

import android.content.Context;
import android.graphics.*;
import android.graphics.drawable.GradientDrawable;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.Switch;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.ui.dashboard.viewholders.button.ButtonViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewholders.image.ImageViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewholders.quantity.QuantityViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewholders.switchy.ReadOnlySwitchViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewholders.switchy.SwitchViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewholders.text.TextViewHolder;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.SensorViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.button.ButtonViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.image.ImageViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.quantity.QuantityViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.switchy.SwitchViewState;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.switchy.SwitchViewModel;
import com.fruithapnotifier.app.ui.dashboard.viewmodels.text.TextViewModel;

import java.util.List;

import static com.fruithapnotifier.app.ui.dashboard.viewmodels.SensorViewModel.*;


public class DashboardAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder>
{
    private List<SensorViewModel> items;
    private Context context;

    public DashboardAdapter(List<SensorViewModel> items)
    {
        this.items = items;
    }

    public List<SensorViewModel> getItems() {
        return items;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType)
    {
        RecyclerView.ViewHolder viewHolder = null;
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());
        context = parent.getContext();

        switch (viewType)
        {
            case VIEWTYPE_SWITCH:
            {
                View switchView = inflater.inflate(R.layout.dashboard_switchitem, parent, false);
                viewHolder = new SwitchViewHolder(switchView);
                break;
            }
            case VIEWTYPE_READONLYSWITCH:
            {
                View switchView = inflater.inflate(R.layout.dashboard_readonlyswitchitem, parent, false);
                viewHolder = new ReadOnlySwitchViewHolder(switchView);
                break;
            }
            case VIEWTYPE_BUTTON:
            {
                View buttonView = inflater.inflate(R.layout.dashboard_buttonitem, parent, false);
                viewHolder = new ButtonViewHolder(buttonView);
                break;
            }
            case VIEWTYPE_UNITVALUE:
            {
                View quantityView = inflater.inflate(R.layout.dashboard_quantityitem, parent, false);
                viewHolder = new QuantityViewHolder(quantityView);
                break;
            }
            case VIEWTYPE_IMAGEVALUE:
            {
                View imageView = inflater.inflate(R.layout.dashboard_imageitem, parent, false);
                viewHolder = new ImageViewHolder(imageView);
                break;
            }

            case VIEWTYPE_TEXTVALUE:
            {
                View textView = inflater.inflate(R.layout.dashboard_textitem, parent, false);
                viewHolder = new TextViewHolder(textView);
                break;
            }
        }

        return viewHolder;
    }

    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {

        switch (holder.getItemViewType())
        {
            case VIEWTYPE_SWITCH:
            {
                SwitchViewHolder switchViewHolder = (SwitchViewHolder) holder;
                configureSwitchViewHolder(switchViewHolder, position);
                break;
            }
            case VIEWTYPE_READONLYSWITCH:
            {
                ReadOnlySwitchViewHolder switchViewHolder = (ReadOnlySwitchViewHolder) holder;
                configureReadOnlySwitchViewHolder(switchViewHolder, position);
                break;
            }
            case VIEWTYPE_BUTTON:
            {
                ButtonViewHolder buttonViewHolder = (ButtonViewHolder) holder;
                configureButtonViewHolder(buttonViewHolder, position);
                break;
            }
            case VIEWTYPE_UNITVALUE:
            {
                QuantityViewHolder quantityViewHolder = (QuantityViewHolder) holder;
                configureQuantityViewHolder(quantityViewHolder, position);
                break;
            }
            case VIEWTYPE_IMAGEVALUE:
            {
                ImageViewHolder imageViewHolder = (ImageViewHolder)holder;
                configureImageViewHolder(imageViewHolder, position);
                break;
            }
            case VIEWTYPE_TEXTVALUE:
            {
                TextViewHolder textViewHolder = (TextViewHolder)holder;
                configureTextViewHolder(textViewHolder, position);
                break;
            }
        }
    }

    private void configureTextViewHolder(TextViewHolder textViewHolder, int position)
    {
        final TextViewModel item = (TextViewModel)items.get(position);
        if (item != null) {
            textViewHolder.getTxtName().setText(item.getName());
            textViewHolder.getTxtDesc().setText(item.getDescription());
            textViewHolder.getTxtLastupdated().setText(item.getLastUpdated());
            textViewHolder.getTxtValue().setText(item.getValue());
        }

    }

    private void configureImageViewHolder(ImageViewHolder imageViewHolder, int position)
    {
        final ImageViewModel item = (ImageViewModel)items.get(position);
        if (item != null)
        {
            imageViewHolder.getTxtName().setText(item.getName());
            imageViewHolder.getTxtDesc().setText(item.getDescription());
            imageViewHolder.getTxtLastupdated().setText(item.getLastUpdated());

            byte[] imageBytes = item.getImageData();
            ImageView imageView = imageViewHolder.getImgImage();
            if ( imageBytes != null)
            {
                Bitmap decodedImage = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
                if (decodedImage == null)
                {
                    Bitmap bmp = getNABitmap();
                    imageView.setImageBitmap(bmp);
                }
                else
                {
                    imageView.setImageBitmap(decodedImage);
                }
            }
            else
            {
                Bitmap bmp = getNABitmap();
                imageView.setImageBitmap(bmp);
            }

        }
    }

    private Bitmap getNABitmap()
    {
        Bitmap bmp = Bitmap.createBitmap(150,150, Bitmap.Config.ARGB_8888);
        Canvas c = new Canvas(bmp);
        Paint p = new Paint();
        p.setColor(Color.RED);
        p.setStrokeWidth(5);
        c.drawLine(0,0,150,150,p);
        c.drawLine(150,0,0,150,p);
        return bmp;
    }

    private void configureQuantityViewHolder(QuantityViewHolder quantityViewHolder, int position)
    {
        final QuantityViewModel item = (QuantityViewModel)items.get(position);
        if (item != null)
        {
            quantityViewHolder.getTxtName().setText(item.getName());
            quantityViewHolder.getTxtDesc().setText(item.getDescription());
            quantityViewHolder.getTxtLastupdated().setText(item.getLastUpdated());
            quantityViewHolder.getTxtUnit().setText(item.getUnitText());
            if (Double.isNaN(item.getValue()))
            {
                quantityViewHolder.getTxtValue().setText("N/A");
            }
            else
            {
                quantityViewHolder.getTxtValue().setText(Double.toString(item.getValue()));
            }
        }
    }

    private void configureButtonViewHolder(ButtonViewHolder buttonViewHolder, int position)
    {
        final ButtonViewModel item = (ButtonViewModel)items.get(position);
        if (item != null)
        {
            buttonViewHolder.getTxtName().setText(item.getName());
            buttonViewHolder.getTxtDesc().setText(item.getDescription());
        }

        buttonViewHolder.getBtnExecute().setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                item.press();

            }
        });
    }

    private void configureReadOnlySwitchViewHolder(ReadOnlySwitchViewHolder switchViewHolder, int position)
    {
        final SwitchViewModel item = (SwitchViewModel)items.get(position);
        if (item != null)
        {
            switchViewHolder.getTxtName().setText(item.getName());
            switchViewHolder.getTxtDesc().setText(item.getDescription());
            switchViewHolder.getTxtLastupdated().setText(item.getLastUpdated());
            GradientDrawable circle = (GradientDrawable) switchViewHolder.getSwState().getDrawable();
            int color = item.getState() == SwitchViewState.ON ? Color.rgb(0xAA,0,0) : Color.TRANSPARENT;
            circle.setColor(color);
        }
    }

    private void configureSwitchViewHolder(SwitchViewHolder switchViewHolder, final int position)
    {
        final SwitchViewModel item = (SwitchViewModel)items.get(position);
        if (item != null)
        {
            switchViewHolder.getTxtName().setText(item.getName());
            switchViewHolder.getTxtDesc().setText(item.getDescription());
            switchViewHolder.getTxtLastupdated().setText(item.getLastUpdated());

            switchViewHolder.getSwState().setChecked(item.getState() == SwitchViewState.ON);

            switchViewHolder.getSwState().setOnClickListener(new View.OnClickListener()
            {
                @Override
                public void onClick(View view)
                {
                    Switch theSwitch = (Switch)view;
                    if (theSwitch.isChecked())
                    {
                        item.setState(SwitchViewState.ON,true);
                    }
                    else
                    {
                        item.setState(SwitchViewState.OFF,true);
                    }

                }
            });
        }
    }

    @Override
    public int getItemViewType(int position)
    {
        return items.get(position).getViewType();
    }

    @Override
    public int getItemCount()
    {
        return items.size();
    }
}
