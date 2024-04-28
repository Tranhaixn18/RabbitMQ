using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternateExchangeDemo
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

            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
            channel.ExchangeDeclare("ex.direct", "direct", true, false, new Dictionary<string, object>()
            {
                {"alternate-exchange","ex.fanout" }
            });

            channel.QueueDeclare("queue1", true, false, false, null);
            channel.QueueDeclare("queue2", true, false, false, null);
            channel.QueueDeclare("unrouted", true, false, false, null);

            channel.QueueBind("queue1", "ex.direct", "video");
            channel.QueueBind("queue2", "ex.direct", "image");
            channel.QueueBind("unrouted", "ex.fanout", "");

            channel.BasicPublish("ex.direct", "video", null, Encoding.UTF8.GetBytes("Message has routing key video"));
            channel.BasicPublish("ex.direct", "text", null, Encoding.UTF8.GetBytes("Message has not routing key"));
        }
    }
}
