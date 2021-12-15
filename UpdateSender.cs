using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using BackOffice.Messaging.Send.Options;

namespace BackOffice.Messaging.Send
{
    public class UpdateSender : IUpdateSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private IConnection _connection;
        private readonly string _exchangeName;

        public UpdateSender(IOptions<RabbitMqConfiguration> rabbitOptions)
        {
            _queueName = rabbitOptions.Value.QueueName;
            _hostname = rabbitOptions.Value.HostName;
            _username = rabbitOptions.Value.UserName;
            _password = rabbitOptions.Value.Password;
            _exchangeName = rabbitOptions.Value.ExchangeName;

            CreateConnection();
        }

        public void SendEntity<TEntity>(TEntity entity)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    //   channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var entityInJson = JsonConvert.SerializeObject(entity);
                    var body = Encoding.UTF8.GetBytes(entityInJson);

                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}