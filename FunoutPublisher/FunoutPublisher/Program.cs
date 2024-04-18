using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace FunoutPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConnection connection;
            IModel channel;
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";
            connectionFactory.VirtualHost = "/";
            connectionFactory.Port = 5672;
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);

            channel.QueueDeclare("my.queue1",true, false,false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.fanout", "", null);
            channel.QueueBind("my.queue2", "ex.fanout", "", null);

            channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message 2"));

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout");

            channel.Close();
            connection.Close();
        }
    }
}
