using MerQure.Tools.Configurations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses;

public class ConsumerProvider
{
    private static readonly object _syncRoot = new();

    private readonly Dictionary<string, IConsumer> _consumers = new Dictionary<string, IConsumer>();
    private readonly IMessagingService _messagingService;

    public ConsumerProvider(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task<IConsumer> GetAsync(Channel channel)
    {
        // First check if consumer exists (inside lock)
        IConsumer consumer;
        bool consumerExists;

        lock (_syncRoot)
        {
            consumerExists = _consumers.TryGetValue(channel.Value, out consumer);
        }

        // If consumer doesn't exist, create it outside the lock
        if (!consumerExists)
        {
            consumer = await _messagingService.GetConsumerAsync(channel.Value);

            // Add to dictionary with lock protection
            lock (_syncRoot)
            {
                if (!_consumers.ContainsKey(channel.Value))
                {
                    _consumers.Add(channel.Value, consumer);
                }
                else
                {
                    // Another thread may have created the consumer already
                    consumer = _consumers[channel.Value];
                }
            }
        }

        return consumer;
    }
}