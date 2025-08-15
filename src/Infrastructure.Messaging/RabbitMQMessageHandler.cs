using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
namespace StudentSystem.Infrastructure.Messaging;

public class RabbitMQMessageHandler : IMessageHandler
{
    private const int DEFAULT_PORT = 5672;
    private const string DEFAULT_VIRTUAL_HOST = "/";
    
    private readonly List<string> _hosts;
    private readonly string _username;
    private readonly string _virtualHost;
    private readonly string _password;
    private readonly string _exchange;
    private readonly string _queueName;
    private readonly string _routingKey;
    private readonly int _port;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;
    private string _consumerTag;
    private IMessageHandlerCallback _callback;

    public RabbitMQMessageHandler(string host, string username, string password, 
        string exchange, string queueName, string routingKey)
        : this(new List<string> { host }, DEFAULT_VIRTUAL_HOST, username, password, 
            exchange, queueName, routingKey, DEFAULT_PORT)
    {
    }

     public RabbitMQMessageHandler(string host, string virtualHost, string username, string password, 
        string exchange, string queueName, string routingKey)
        : this(new List<string> { host }, virtualHost, username, password, 
            exchange, queueName, routingKey, DEFAULT_PORT)
    {
    }   

    public RabbitMQMessageHandler(string host, string username, string password, 
        string exchange, string queueName, string routingKey, int port)
        : this(new List<string> { host }, DEFAULT_VIRTUAL_HOST, username, password, 
            exchange, queueName, routingKey, port)
    {
    }
    public RabbitMQMessageHandler(string host, string virtualHost, string username, string password, 
        string exchange, string queueName, string routingKey, int port)
        : this(new List<string> { host }, virtualHost, username, password, 
            exchange, queueName, routingKey, port)
    {
    }    

    public RabbitMQMessageHandler(IEnumerable<string> hosts, string username, string password, 
        string exchange, string queueName, string routingKey)
        : this(hosts, DEFAULT_VIRTUAL_HOST, username, password, 
            exchange, queueName, routingKey, DEFAULT_PORT)
    {
    }

    public RabbitMQMessageHandler(IEnumerable<string> hosts, string virtualHost, string username, string password, 
        string exchange, string queueName, string routingKey, int port)
    {
        _hosts = hosts.ToList();
        _virtualHost = virtualHost;
        _port = port;
        _username = username;
        _password = password;
        _exchange = exchange;
        _queueName = queueName;
        _routingKey = routingKey;
    }

    public void Start(IMessageHandlerCallback callback)
    {
        _callback = callback;

        Policy
            .Handle<Exception>()
            .WaitAndRetry(9, r => TimeSpan.FromSeconds(5), (ex, ts) => { Log.Error("Error connecting to RabbitMQ. Retrying in 5 sec."); })
            .Execute(async () =>
            {
                var factory = new ConnectionFactory() { VirtualHost = _virtualHost, UserName = _username, Password = _password, Port = _port };
                _connection = await factory.CreateConnectionAsync(_hosts);
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(_exchange, "fanout", durable: true, autoDelete: false);
                await _channel.QueueDeclareAsync(_queueName, durable: true, autoDelete: false, exclusive: false);
                await _channel.QueueBindAsync(_queueName, _exchange, _routingKey);
                _consumer = new AsyncEventingBasicConsumer(_channel);
                _consumer.ReceivedAsync += Consumer_Received;
                _consumerTag = await _channel.BasicConsumeAsync(_queueName, false, _consumer);
            });
    }

    public async void Stop()
    {
        await _channel.BasicCancelAsync(_consumerTag);
        await _channel.CloseAsync(200, "Goodbye");
        await _connection.CloseAsync();
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs ea)
    {
        if (await HandleEvent(ea))
        {
            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        }
    }

    private Task<bool> HandleEvent(BasicDeliverEventArgs ea)
    {
        string messageType = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["MessageType"]);
        string body = Encoding.UTF8.GetString(ea.Body.ToArray());
        return _callback.HandleMessageAsync(messageType, body);
    }
}