using MerQure;
using MerQure.Tools.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Exchanges
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

        public IConsumer Get(Channel binding)
        {
            IConsumer consumer;
            lock (_syncRoot)
            {
                if (_consumers.ContainsKey(binding))
                {
                    consumer = _consumers[binding];
                }
                else
                {
                    consumer = _messagingService.GetConsumer(binding.Value);
                    _consumers.Add(binding, consumer);
                }
            }
            return consumer;
        }
    }
}
