using System;
using RabbitMQ.Client;
using System.Text;

namespace Send
{
    class Send
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() 
            { 
                HostName = "localhost",
                Port = 5672
            };
            using (var connection = factory.CreateConnection())
            using (var chanel = connection.CreateModel())
            {
                chanel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                string message = "Hello RabbitMQ Tam Tam";
                var body = Encoding.UTF8.GetBytes(message);
                chanel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                Console.WriteLine("[x] Sent {0}", message);
            }
            Console.WriteLine("Press to exit");
            Console.ReadLine();
        }
    }
}
