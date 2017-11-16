using MerQure.Messages;
using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Buses
{
    internal class Consumer<T> where T : IDelivered
    {
        private readonly ConsumerProvider _consumerProvider;
        internal Dictionary<string, RetryInformations> RetryInformations { get; } = new Dictionary<string, RetryInformations>();

        public Consumer(IMessagingService messagingService)
        {
            _consumerProvider = new ConsumerProvider(messagingService);
        }

        public void Consume(Channel channel, EventHandler<T> callback)
        {
            _consumerProvider.Get(channel).Consume((object sender, IMessagingEvent messagingEvent) =>
            {
                RetryMessage<T> retryMessage = JsonConvert.DeserializeObject<RetryMessage<T>>(messagingEvent.Message.GetBody());
                retryMessage.OriginalMessage.DeliveryTag = messagingEvent.DeliveryTag;
                RetryInformations.Add(messagingEvent.DeliveryTag, retryMessage.RetryInformations);

                callback(this, retryMessage.OriginalMessage);
            });
        }

        public void AcknowlegdeDeliveredMessage(Channel channel, IDelivered deliveredMessage)
        {
            _consumerProvider.Get(channel).AcknowlegdeDeliveredMessage(deliveredMessage);
        }

        public void RejectDeliveredMessage(Channel channel, IDelivered deliveredMessage)
        {
            _consumerProvider.Get(channel).RejectDeliveredMessage(deliveredMessage);
        }
    }
}
