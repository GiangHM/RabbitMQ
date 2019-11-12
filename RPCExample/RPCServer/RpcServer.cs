using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RPCServer
{
    public class RpcServer
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
                chanel.QueueDeclare(queue: "rpc_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                chanel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(chanel);
                chanel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");
                consumer.Received += (model, ea) =>
                  {
                      string response = string.Empty;
                      var correlationId = ea.BasicProperties.CorrelationId;
                      var replyQueue = ea.BasicProperties.ReplyTo;
                      var replyProp = chanel.CreateBasicProperties();
                      replyProp.CorrelationId = correlationId;
                      var requestBody = ea.Body;
                      try
                      {
                          var message = Encoding.UTF8.GetString(requestBody);
                          int valueToCalculate = int.Parse(message);
                          Console.WriteLine(" [.] fib({0})", message);
                          response = CalculateFib(valueToCalculate).ToString();
                      }
                      catch (Exception e)
                      {
                          Console.WriteLine(" [.] " + e.Message);
                          response = "";
                      }
                      finally
                      {
                          var responseBody = Encoding.UTF8.GetBytes(response);
                          chanel.BasicPublish(exchange: "", routingKey: replyQueue, basicProperties: replyProp, body: responseBody);
                          chanel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                      }
                  };
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        private static int CalculateFib(int input)
        {
            if (input == 0 || input == 1)
            {
                return input;
            }

            return CalculateFib(input - 1) + CalculateFib(input - 2);
        }
    }
}
