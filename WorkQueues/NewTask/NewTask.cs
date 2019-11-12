using System;
using RabbitMQ.Client;
using System.Text;

namespace NewTask
{
    class NewTask
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };
            using(var connection = factory.CreateConnection())
            using(var chanel = connection.CreateModel())
            {
                chanel.QueueDeclare(queue: "task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = chanel.CreateBasicProperties();
                properties.Persistent = true;

                chanel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);
                Console.WriteLine("[x] sent message {0}", message);
                
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
        private static string GetMessage(string []args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello my World!!!");
        }
    }
}
