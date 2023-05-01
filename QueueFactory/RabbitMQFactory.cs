using RabbitMQ.Client;
using QueueFactory.Models;
using QueueFactory.Models.Interfaces;

namespace QueueFactory;

public class RabbitMQFactory
{
    private static ConnectionFactory _factory;
    private static IConnection _connection;
    private static IModel _channel;

    public static IBus CreateBus(string hostName)
    {
        _factory = new ConnectionFactory() { HostName = hostName, DispatchConsumersAsync = true };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        return new RabbitMQBus(_channel);
    }
}