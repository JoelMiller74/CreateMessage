using System;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;

class NewTask
{
    public static void Main(string[] args)
    {
        //https://leonidius2010.wordpress.com/2017/09/21/reading-appsettings-in-net-core-2-console-application/
        IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", true, true)
          .Build();
        string RabbitMQHost = config["RabbitMQHost"];
        string RabbitMQQueueName = config["RabbitMQQueueName"];
        var factory = new ConnectionFactory() { HostName = RabbitMQHost };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: RabbitMQQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "", routingKey: RabbitMQQueueName, basicProperties: properties, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
