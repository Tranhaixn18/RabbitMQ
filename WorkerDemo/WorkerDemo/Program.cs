using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerDemo
{
    internal class Program
    {
        static IConnection connection;
        static IModel channel;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the name for this worker");
            string workerName = Console.ReadLine();
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            // BasicQos: thiết lập mỗi consumer không nhận quá tin nhắn cùng 1 lúc thông qua prefetchcount
            channel.BasicQos(0,1,false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                int durationInSecond = Int32.Parse(message);
                Console.WriteLine($"{workerName} Task started. Duration: {durationInSecond}");

                Thread.Sleep(durationInSecond * 1000);
                Console.WriteLine("FINISHED");
                channel.BasicAck(e.DeliveryTag, false);
            };

            var consumerTag = channel.BasicConsume("my.queue1", false, consumer);

            Console.WriteLine("Waiting for message. Press a key to exit");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }
    }
}
