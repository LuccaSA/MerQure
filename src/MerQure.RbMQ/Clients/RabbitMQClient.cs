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

        protected RabbitMqClient(IModel channel): this(channel, false)
        {
        }

        protected RabbitMqClient(IModel channel, bool enablePublisherAcknowledgements)
        {
            Channel = channel;
            if(enablePublisherAcknowledgements)
            {
                channel.ConfirmSelect();
            }
        }

        public void Dispose()
        {
            Channel?.Close();
        }
    }
}
