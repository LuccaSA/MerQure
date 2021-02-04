using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Net.Security;

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
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled,
                ClientProvidedName = rabbitMqConnection.FriendlyName,
                RequestedChannelMax = rabbitMqConnection.RequestedChannelMax
            };

            if (rabbitMqConnection.ConnectionCluster != null && !string.IsNullOrEmpty(rabbitMqConnection.ConnectionString))
            {
                throw new ArgumentException("MerQureConnection could not provide a ConnectionCluster or ConnectionString");
            }

            if (rabbitMqConnection.ConnectionCluster is null)
            {
                connectionFactory.Uri = new Uri(rabbitMqConnection.ConnectionString);
                return connectionFactory.CreateConnection();
            }
            InitCluster(connectionFactory, rabbitMqConnection.ConnectionCluster);
            return connectionFactory.CreateConnection();

        }

        private static void InitCluster(ConnectionFactory connectionFactory, MerQureConnectionCluster rabbitMqConnectionCluster)
        {
            connectionFactory.VirtualHost = rabbitMqConnectionCluster.VirtualHost;
            connectionFactory.UserName = rabbitMqConnectionCluster.UserName;
            connectionFactory.Password = rabbitMqConnectionCluster.Password;

            if (rabbitMqConnectionCluster.Ssl)
            {
                connectionFactory.Ssl.Enabled = true;
                connectionFactory.Ssl.Version = connectionFactory.AmqpUriSslProtocols;
                connectionFactory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch;
            }
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
