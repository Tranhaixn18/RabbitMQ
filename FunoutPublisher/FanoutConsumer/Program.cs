using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutConsumer
{
    internal class Program
    {
        static IConnection connection;
        static IModel channel;
        static void Main(string[] args)
        {
            
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";
            connectionFactory.VirtualHost = "/";
            connectionFactory.Port = 5672;
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Recieved;

            // true: message ở trạng thái ready
            // false: message ở trạng thái unasknowledge
            var consumerTag = channel.BasicConsume("my.queue1", false, consumer);
            
            Console.WriteLine("Waiting for message. Press any key to exit");
            Console.ReadKey();
        }

        private static void Consumer_Recieved(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine("Message: "+ message);

            // báo với Rabbit rằng consumer đã xử lý và bạn có thể xoá message đi
            //channel.BasicAck(e.DeliveryTag, false);
            // từ trối message 
            //channel.BasicNack(e.DeliveryTag, false, true);
        }
    }
}
