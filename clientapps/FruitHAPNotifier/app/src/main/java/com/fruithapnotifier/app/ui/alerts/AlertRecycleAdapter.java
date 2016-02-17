package com.fruithapnotifier.app.ui.alerts;

import android.graphics.drawable.GradientDrawable;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import com.fruithapnotifier.app.R;
import com.fruithapnotifier.app.domain.Alert;
import com.fruithapnotifier.app.persistence.AlertRepository;
import com.fruithapnotifier.app.ui.helpers.PriorityHelpers;
import org.joda.time.format.DateTimeFormat;
import org.json.JSONException;

import java.util.List;

/**
 * Created by developer on 1/26/16.
 */
@Deprecated
public class AlertRecycleAdapter extends RecyclerView.Adapter<AlertRecycleAdapter.AlertViewHolder>
{
    private final AlertRepository repository;
    private List<Alert> alertList;

    public AlertRecycleAdapter(List<Alert> alertList, AlertRepository repository)
    {
        this.alertList = alertList;
        this.repository = repository;
    }


    public void insertItem(Alert alert)
    {
        alertList.add(alert);

        notifyItemInserted(alertList.size() - 1);

    }

    @Override
    public int getItemCount() {
        return alertList.size();
    }

    @Override
    public void onBindViewHolder(AlertViewHolder alertViewHolder, int i)
    {
        try
        {
            Alert event = alertList.get(i);
            if (event != null)
            {
                GradientDrawable background = (GradientDrawable) alertViewHolder.txtPriority.getBackground();
                background.setColor(PriorityHelpers.convertToColor(event.getNotificationPriority()));
                alertViewHolder.txtPriority.setText(PriorityHelpers.getTextResource(event.getNotificationPriority()));
                alertViewHolder.txtTimestamp.setText(DateTimeFormat.forStyle("SL").print(event.getTimestamp()));
                alertViewHolder.txtMessage.setText(event.getNotificationText());
            }
        }
        catch (Exception e)
        {
            Log.e(this.getClass().getName(),"Error:",e);
        }
    }

    @Override
    public AlertViewHolder onCreateViewHolder(ViewGroup viewGroup, int i) {
        View itemView = LayoutInflater.
                from(viewGroup.getContext()).
                inflate(R.layout.alert_listitem, viewGroup, false);

        return new AlertViewHolder(itemView);
    }

    public void onItemDeleted(Alert alert)
    {
        int alertPosition = alertList.indexOf(alert);
        if (alertPosition != -1)
        {
            alertList.remove(alert);
            notifyItemRemoved(alertPosition);
        }
    }

    public void clearList()
    {
        alertList.clear();
        notifyDataSetChanged();
    }

    public void deleteFromList(int adapterPosition)
    {
        repository.deleteAlert(alertList.get(adapterPosition));
    }


    public static class AlertViewHolder extends RecyclerView.ViewHolder
    {
        /**/

        protected TextView txtPriority;
        protected TextView txtTimestamp;
        protected TextView txtMessage;

        public AlertViewHolder(View v) {
            super(v);
            txtPriority = (TextView) v.findViewById(R.id.alert_listitem_txtPriority);
            txtTimestamp = (TextView) v.findViewById(R.id.alert_listitem_txtTimestamp);
            txtMessage = (TextView) v.findViewById(R.id.alert_listitem_txtMessage);
        }
    }
}

/*public class ContactAdapter extends RecyclerView.Adapter<ContactAdapter.ContactViewHolder> {

    private List<ContactInfo> alertList;

    public ContactAdapter(List<ContactInfo> alertList) {
        this.alertList = alertList;
    }


    @Override
    public int getItemCount() {
        return alertList.size();
    }

    @Override
    public void onBindViewHolder(ContactViewHolder contactViewHolder, int i) {
        ContactInfo ci = alertList.get(i);
        contactViewHolder.vName.setText(ci.name);
        contactViewHolder.vSurname.setText(ci.surname);
        contactViewHolder.vEmail.setText(ci.email);
        contactViewHolder.vTitle.setText(ci.name + " " + ci.surname);
    }

    @Override
    public ContactViewHolder onCreateViewHolder(ViewGroup viewGroup, int i) {
        View itemView = LayoutInflater.
                from(viewGroup.getContext()).
                inflate(R.layout.card_detail_layout, viewGroup, false);

        return new ContactViewHolder(itemView);
    }

    public static class ContactViewHolder extends RecyclerView.ViewHolder {

        protected TextView vName;
        protected TextView vSurname;
        protected TextView vEmail;
        protected TextView vTitle;

        public ContactViewHolder(View v) {
            super(v);
            vName =  (TextView) v.findViewById(R.id.txtName);
            vSurname = (TextView)  v.findViewById(R.id.txtSurname);
            vEmail = (TextView)  v.findViewById(R.id.txtEmail);
            vTitle = (TextView) v.findViewById(R.id.title);
        }
    }
}*/
