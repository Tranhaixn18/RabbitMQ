using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
namespace TopicExchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConnection connection;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare("ex.topic", "topic", true, false, null);

            channel.QueueDeclare("my.queue1",true,false,false, null);
            channel.QueueDeclare("my.queue2",true,false,false, null);
            channel.QueueDeclare("my.queue3",true,false,false, null);

            channel.QueueBind("my.queue1", "ex.topic", "*.image.*");
            channel.QueueBind("my.queue2", "ex.topic", "#.image");
            channel.QueueBind("my.queue3", "ex.topic", "image.#");

            channel.BasicPublish("ex.topic", "conver.image.bmp", null, Encoding.UTF8.GetBytes("Routing key is conver.image.bmp"));
            channel.BasicPublish("ex.topic", "conver.bitmap.image", null, Encoding.UTF8.GetBytes("Routing key is conver.bitmap.image"));
            channel.BasicPublish("ex.topic", "image.bipmap.32bit", null, Encoding.UTF8.GetBytes("Routing key is image.bipmap.32bit"));
        }
    }
}
