using System;
using RabbitMQ.Client;

namespace MerQure.RbMQ.Clients
{
    public abstract class RabbitMqClient : IDisposable
    {
        /// <summary>
        /// rabbitmq-dotnet-client key objet
        /// </summary>
        public IModel Channel { get; set; }

        protected RabbitMqClient(IModel channel)
        {
            Channel = channel;
        }

        public void Dispose()
        {
            Channel?.Close();
        }
    }
}
