using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace MerQure.RbMQ
{
    /// <summary>
    /// Must be registered as a singleton
    /// </summary>
    public class SharedConnection
    {
        private readonly RabbitMqConnectionConfigurationElement _config;
        private readonly Lazy<IConnection> _currentConnection;

        public SharedConnection(IOptions<RabbitMqConnectionConfigurationElement> options)
        {
            _config = options.Value;
            _currentConnection = new Lazy<IConnection>(() => GetRabbitMqConnection(_config));
        }

        public IConnection CurrentConnection => _currentConnection.Value;

        private static IConnection GetRabbitMqConnection(RabbitMqConnectionConfigurationElement rabbitMqConnection)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnection.ConnectionString),
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled
            };
            return connectionFactory.CreateConnection();
        }
    }
}
