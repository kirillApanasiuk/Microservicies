using PlatformService.Dtos;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();

                _channel = _connection.CreateModel();


                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMqConnectionShotdown;

                Console.WriteLine("--> Connected to the message bus");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not connect to the Message bus: {ex.Message}");
            }
        }

        private void RabbitMqConnectionShotdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine(" --> RabbitMQ connection was shotdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMq Connection Open, sending messages ...");

                SendMessage(message);

            }
            else
            {
                Console.WriteLine("--> RabbitMq connections closed, not  sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> Message have sent {body}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus disposed");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
