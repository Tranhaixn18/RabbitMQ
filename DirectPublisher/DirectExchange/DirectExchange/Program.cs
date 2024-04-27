using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace DirectExchange
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

            channel.ExchangeDeclare("ex.direct", "direct", true, false, null);
            channel.QueueDeclare("my.infos", true, false,false, null);
            channel.QueueDeclare("my.warrings", true, false,false, null);
            channel.QueueDeclare("my.errors", true, false,false, null);

            channel.QueueBind("my.infos", "ex.direct", "info");
            channel.QueueBind("my.warrings", "ex.direct", "warring");
            channel.QueueBind("my.errors", "ex.direct", "error");

            channel.BasicPublish("ex.direct", "info",null,Encoding.UTF8.GetBytes("this is a infomaton"));
            channel.BasicPublish("ex.direct", "warring", null,Encoding.UTF8.GetBytes("this is a warring"));
            channel.BasicPublish("ex.direct", "error", null,Encoding.UTF8.GetBytes("this is an error"));
        }
    }
}
