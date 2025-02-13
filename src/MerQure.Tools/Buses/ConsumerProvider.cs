using MerQure.Tools.Configurations;
using System.Collections.Generic;

namespace MerQure.Tools.Buses
{
	public class ConsumerProvider
    {
        private static readonly object _syncRoot = new();

        private readonly Dictionary<string, IConsumer> _consumers = new Dictionary<string, IConsumer>();
        private readonly IMessagingService _messagingService;

        public ConsumerProvider(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public IConsumer Get(Channel channel)
        {
            IConsumer consumer;
            lock (_syncRoot)
            {
                if (_consumers.TryGetValue(channel.Value, out var foundConsumer))
                {
                    consumer = foundConsumer;
                }
                else
                {
                    consumer = _messagingService.GetConsumer(channel.Value);
                    _consumers.Add(channel.Value, consumer);
                }
            }
            return consumer;
        }
    }
}
