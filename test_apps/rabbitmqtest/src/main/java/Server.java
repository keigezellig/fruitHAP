import com.rabbitmq.client.*;


/**
 * Created by MJOX03 on 8.12.2015.
 */
public class Server
{
    private static final String RPC_QUEUE_NAME = "rpc_queue";

    public static void main(String[] args)
    {

        ConnectionFactory factory = new ConnectionFactory();
        factory.setHost("192.168.56.102");
        factory.setUsername("admin");
        factory.setPassword("admin");

        try
        {
            Connection connection = factory.newConnection();
            Channel channel = connection.createChannel();
            String pubsubmsg = "Hello from test";
            channel.basicPublish("pubsubExchange", "test", null, pubsubmsg.getBytes());


            channel.exchangeDeclare("rpcExchange","direct");
            channel.queueDeclare(RPC_QUEUE_NAME, false, false, false, null);
            channel.queueBind(RPC_QUEUE_NAME,"rpcExchange",RPC_QUEUE_NAME);

            channel.basicQos(1);

            QueueingConsumer consumer = new QueueingConsumer(channel);
            channel.basicConsume(RPC_QUEUE_NAME, false, consumer);



            System.out.println(" [x] Awaiting RPC requests");

            while (true) {
                QueueingConsumer.Delivery delivery = consumer.nextDelivery();
                AMQP.BasicProperties props = delivery.getProperties();
                AMQP.BasicProperties replyProps = new AMQP.BasicProperties()
                        .builder()
                        .correlationId(props.getCorrelationId())
                        .build();


                String message = new String(delivery.getBody());
                int n = Integer.parseInt(message);

                System.out.println(" [.] fib(" + message + ")");
                String response = "" + fib(n);

                channel.basicPublish("", props.getReplyTo(), replyProps, response.getBytes());

                channel.basicAck(delivery.getEnvelope().getDeliveryTag(), false);
            }
        }
            catch (Exception ex)
            {
                ex.printStackTrace();
            }
        }

    private static int fib(int n) throws Exception
    {
        if (n == 0) return 0;
        if (n == 1) return 1;
        return fib(n-1) + fib(n-2);
    }

}
