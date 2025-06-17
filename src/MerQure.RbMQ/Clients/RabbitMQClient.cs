using System;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Clients;

internal abstract class RabbitMqClient : IAsyncDisposable
{
    /// <summary>
    /// rabbitmq-dotnet-client key objet
    /// </summary>
    public IChannel Channel { get; set; }

    protected RabbitMqClient(IChannel channel)
    {
        Channel = channel;
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Channel?.CloseAsync());
    }
}