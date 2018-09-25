﻿using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace MerQure.RbMQ
{
    /// <summary>
    /// Must be registered as a singleton
    /// </summary>
    public class SharedConnection : IDisposable
    {
        private readonly MerQureConnection _config;
        private readonly Lazy<IConnection> _currentConnection;

        public SharedConnection(IOptions<MerQureConfiguration> options)
        {
            _config = options.Value.Connection;
            _currentConnection = new Lazy<IConnection>(() => GetRabbitMqConnection(_config));
        }

        public IConnection CurrentConnection => _currentConnection.Value;

        private static IConnection GetRabbitMqConnection(MerQureConnection rabbitMqConnection)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnection.ConnectionString),
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled
            };
            return connectionFactory.CreateConnection();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (_currentConnection.IsValueCreated)
            {
                _currentConnection.Value.Dispose();
            }
        }
    }
}
