using MerQure;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses
{
    internal class ConsumerProvider
    {
        private static object _syncRoot = new Object();

        private Dictionary<Channel, IConsumer> _consumers = new Dictionary<Channel, IConsumer>();
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
                if (_consumers.ContainsKey(channel))
                {
                    consumer = _consumers[channel];
                }
                else
                {
                    consumer = _messagingService.GetConsumer(channel.Value);
                    _consumers.Add(channel, consumer);
                }
            }
            return consumer;
        }
    }
}
