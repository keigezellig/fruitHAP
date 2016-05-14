import com.myown.rabbitmqtest.common.IMQProvider;
import com.myown.rabbitmqtest.common.IMQSubscriptionReceiver;
import com.myown.rabbitmqtest.mqproviders.RabbitMQProvider;
import org.json.simple.JSONObject;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

/**
 * Created by MJOX03 on 8.12.2015.
 */
public class Client
{
    /*QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.ConfigurationMessage");
    QString messageType("FruitHAP.Core.Action.ConfigurationMessage:FruitHAP.Core");
      QJsonObject obj;
    QJsonObject paramObj;

    obj["OperationName"] = "GetAllSensors";
    obj["Parameters"] = paramObj;
    obj["MessageType"] = 0;

    FruitHAP_RpcExchange

    QString routingKey("FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage");
    QString messageType("FruitHAP.Core.Action.SensorMessage:FruitHAP.Core");


*/
    public static void main(String[] args)
    {
        final IMQProvider provider = new RabbitMQProvider("31.151.51.250",9999,"admin","admin",null,"FruitHAP_RpcExchange","FruitHAP_PubSubExchange");
        final IMQSubscriptionReceiver receiver = new IMQSubscriptionReceiver() {
            public void onMessageReceived(String topic, String message)
            {
                System.out.println("PUBSUB: Received message from "+topic + " : "+message);
            }
        };

        try
        {
            provider.subscribe(new String[] {"alerts","SensorMessage"});
            Thread thread = new Thread(new Runnable() {
                public void run() {
                    while (true) {
                        try {
                            provider.processMessages(receiver);
                        } catch (IOException e) {
                            e.printStackTrace();
                            break;
                        } catch (TimeoutException e) {
                            e.printStackTrace();
                            break;
                        }
                    }

                }
            });

            thread.start();


                String routingKey = "FruitHAP_RpcQueue.FruitHAP.Core.Action.ConfigurationMessage";
                String messageType = "FruitHAP.Core.Action.ConfigurationMessage:FruitHAP.Core";
                JSONObject obj = new JSONObject();
                JSONObject paramObj = new JSONObject();
                obj.put("OperationName", "GetAllSensors");
                obj.put("Parameters", paramObj);
                obj.put("MessageType", 0);

                String response = provider.sendRequest(obj.toJSONString(), routingKey, messageType);

                System.out.println("RPC: [.] Got '" + response + "'");

                routingKey = "FruitHAP_RpcQueue.FruitHAP.Core.Action.SensorMessage";
                messageType = "FruitHAP.Core.Action.SensorMessage:FruitHAP.Core";
                obj = new JSONObject();
                obj.put("SensorName", "PIR");
                obj.put("EventType", "GetValue");

                response = provider.sendRequest(obj.toJSONString(), routingKey, messageType);

                System.out.println("RPC: [.] Got '" + response + "'");


            while (true) {

            }
            //provider.close();

        }
        catch (Exception ex)
        {
            ex.printStackTrace();
        }
    }
}
