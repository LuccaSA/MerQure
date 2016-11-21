using RabbitMQ.Client;

namespace MerQure.RbMQ.Clients
{
    abstract class RabbitMQClient
    {
        // rabbitmq-dotnet-client key objet
        public IModel Channel { get; set; }

        public RabbitMQClient(IModel channel)
        {
            this.Channel = channel;
        }
    }
}
