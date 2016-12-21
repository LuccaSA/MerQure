using System;
using RabbitMQ.Client;

namespace MerQure.RbMQ.Clients
{
    internal abstract class RabbitMqClient : IDisposable
    {
        /// <summary>
        /// rabbitmq-dotnet-client key objet
        /// </summary>
        public IModel Channel { get; set; }

        protected RabbitMqClient(IModel channel)
        {
            this.Channel = channel;
        }

        public void Dispose()
        {
            Channel?.Dispose();
        }
    }
}
