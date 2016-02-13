package com.myown.rabbitmqtest;

import com.myown.rabbitmqtest.common.IMQProvider;
import com.myown.rabbitmqtest.mqproviders.RabbitMQProvider;
import com.rabbitmq.client.Channel;
import com.rabbitmq.client.ConnectionFactory;
import org.joda.time.DateTime;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;
import org.json.simple.JSONObject;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;
import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by MJOX03 on 1.2.2016.
 */
public class MessageGenerator
{
    public static void main(String[] args)
    {
//        String uri = args[0];
//        String exchange = args[1];
//        String message = args[2];
//        int prio = Integer.parseInt(args[3]);
//        String topic = args[4];
//        String imgFile = args[5];
//        int delay = Integer.parseInt(args[6]);

        final IMQProvider mqProvider = new RabbitMQProvider("amqp://guest:guest@localhost","","FruitHAP_PubSubExchange");

        Timer timer = new Timer();
          timer.scheduleAtFixedRate(new TimerTask() {
            @Override
            public void run()
            {
                //System.out.println("hoi");
                generateAlerts("TestSensor","High Alert!!",2,"alerts",null,mqProvider);
            }
        },20000,20000);


    }


    public static void generateAlerts(String sensorName, String message, int priority, String topic, byte[] image, IMQProvider mqProvider)
    {
        String request;
            try
            {
                JSONObject dataObject = new JSONObject();
                dataObject.put("NotificationText",message);
                dataObject.put("Priority",priority);
                dataObject.put("OptionalData",image);

                JSONObject header = new JSONObject();
                DateTimeFormatter fmt = ISODateTimeFormat.dateTime();

                DateTime timestamp = new DateTime();
                header.put("TimeStamp",timestamp.toString(fmt));
                header.put("SensorName",sensorName);
                header.put("Data",dataObject);
                header.put("EventType","SensorEvent");

                request = header.toJSONString();
                System.out.printf("Publishing message %s to topic %s\n",request,topic);


                mqProvider.publishMessage(request, topic);
            } catch (Exception ex)
            {
                ex.printStackTrace();
            }




    }
}
