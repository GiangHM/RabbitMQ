using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Text;


namespace Worker
{
    class Worker
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
                chanel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                Console.WriteLine("[*] waiting for message");
                var consumer = new EventingBasicConsumer(chanel);
                consumer.Received += (model, ea) =>
                {
                     var body = ea.Body;
                     var message = Encoding.UTF8.GetString(body);
                     Console.WriteLine("[*] Recieved message {0}", message);
                     int dots = message.Split('.').Length - 1;
                     Thread.Sleep(dots * 1000);
                     Console.WriteLine("[x] done");
                     chanel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                chanel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
