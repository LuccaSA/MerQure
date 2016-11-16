using RabbitMQ.Client;

namespace MerQure.RMQ.Clients
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
