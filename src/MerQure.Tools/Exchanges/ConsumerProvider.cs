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
        private Dictionary<Channel, IConsumer> _consumers;
        private readonly IMessagingService _messagingService;

        public ConsumerProvider(IMessagingService messagingService)
        {
            _messagingService = messagingService;
            _consumers = new Dictionary<Channel, IConsumer>();
        }

        public IConsumer Get(Channel binding)
        {
            IConsumer consumer;
            lock (_consumers)
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
