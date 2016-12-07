using RabbitMQ.Client;

namespace MerQure.RbMQ.Clients
{
    abstract class RabbitMqClient
    {
        // rabbitmq-dotnet-client key objet
        public IModel Channel { get; set; }

        public RabbitMqClient(IModel channel)
        {
            this.Channel = channel;
        }
    }
}
