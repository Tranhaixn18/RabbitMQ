using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PushPullDemo
{
    internal class Program
    {
        static IConnection connection;
        static IModel channel;
        static void Main(string[] args)
        {
            

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            // from queue push message to consumer
            //ReadMessageWithPushModel();
            // from consumer pull message to queue(not recomment)
            ReadMessageWithPullModel();
            channel.Close();
            connection.Close();
        }

        private static void ReadMessageWithPushModel()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Message:" + message);
            };

            string consumerTag = channel.BasicConsume("queue1", true, consumer);

            Console.WriteLine("Subscribed. Press a key to unsubcribe and exit");
            Console.ReadKey();

            channel.BasicCancel(consumerTag);

        }

        private static void ReadMessageWithPullModel()
        {
            Console.WriteLine("Reading message from queue. Press 'e' to exit");

            while (true)
            {
                Console.WriteLine("Try to get a message from the queue...");
                BasicGetResult result = channel.BasicGet("queue1", true);
                if (result != null)
                {
                    string message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine("Message:"+ message);
                }
                if (Console.KeyAvailable)
                {
                    var keyInfor = Console.ReadKey();
                    if (keyInfor.KeyChar == 'e' || keyInfor.KeyChar == 'E')
                    {
                        return;
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}
