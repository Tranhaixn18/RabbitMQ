using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace HeaderPublisher
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

            channel.ExchangeDeclare("ex.headers", "headers", true, false, null);
            channel.QueueDeclare("my.queue1",true,false,false,null);
            channel.QueueDeclare("my.queue2",true,false,false,null);

            channel.QueueBind(
                "my.queue1",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","all" },
                    {"job", "convert" },
                    {"format","jpeg" },
                });
            channel.QueueBind(
                "my.queue2",
                "ex.headers",
                "",
                new Dictionary<string, object>()
                {
                    {"x-match","any" },
                    {"job", "convert" },
                    {"format","jpeg" },
                });

            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "jpeg");

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 1"));

            props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("job", "convert");
            props.Headers.Add("format", "bitmap");

            channel.BasicPublish(
                "ex.headers",
                "",
                props,
                Encoding.UTF8.GetBytes("Message 2"));
        }
    }
}
