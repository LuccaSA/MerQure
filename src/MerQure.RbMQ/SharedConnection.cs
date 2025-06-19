using MerQure.RbMQ.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Net.Security;
using System.Threading.Tasks;

namespace MerQure.RbMQ
{
    /// <summary>
    /// Must be registered as a singleton
    /// </summary>
    public class SharedConnection : IDisposable
    {
        private readonly MerQureConnection _config;
        private readonly Lazy<Task<IConnection>> _currentConnection;

        public SharedConnection(IOptions<MerQureConfiguration> options)
        {
            _config = options.Value.Connection;
            _currentConnection = new Lazy<Task<IConnection>>(() => GetRabbitMqConnectionAsync(_config));
        }

        public Task<IConnection> CurrentConnection => _currentConnection.Value;

        private static Task<IConnection> GetRabbitMqConnectionAsync(MerQureConnection rabbitMqConnection)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = rabbitMqConnection.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = rabbitMqConnection.TopologyRecoveryEnabled,
                ClientProvidedName = rabbitMqConnection.FriendlyName,
                RequestedChannelMax = rabbitMqConnection.RequestedChannelMax
            };

            if (rabbitMqConnection.ConnectionCluster == null && string.IsNullOrEmpty(rabbitMqConnection.ConnectionString))
            {
                throw new ArgumentException("MerQureConnection should provide a ConnectionCluster or ConnectionString");
            }

            if (rabbitMqConnection.ConnectionCluster != null && !string.IsNullOrEmpty(rabbitMqConnection.ConnectionString))
            {
                throw new ArgumentException("MerQureConnection could not provide a ConnectionCluster and ConnectionString at the same time");
            }

            if (rabbitMqConnection.ConnectionCluster is null)
            {
                connectionFactory.Uri = new Uri(rabbitMqConnection.ConnectionString);
                return connectionFactory.CreateConnectionAsync();
            }
            InitCluster(connectionFactory, rabbitMqConnection.ConnectionCluster);
            return connectionFactory.CreateConnectionAsync(rabbitMqConnection.ConnectionCluster.Nodes);
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