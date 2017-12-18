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
                OnMessageReceived(callback, messagingEvent);
            });
        }

        public void AcknowlegdeDeliveredMessage(Channel channel, IDelivered deliveredMessage)
        {
            deliveredMessage.DeliveryTag = DecodeDeliveryTag(deliveredMessage.DeliveryTag);
            _consumerProvider.Get(channel).AcknowlegdeDeliveredMessage(deliveredMessage);
        }

        public void RejectDeliveredMessage(Channel channel, IDelivered deliveredMessage)
        {
            deliveredMessage.DeliveryTag = DecodeDeliveryTag(deliveredMessage.DeliveryTag);
            _consumerProvider.Get(channel).RejectDeliveredMessage(deliveredMessage);
        }

        internal void OnMessageReceived(EventHandler<T> callback, IMessagingEvent messagingEvent)
        {
            RetryMessage<T> retryMessage = JsonConvert.DeserializeObject<RetryMessage<T>>(messagingEvent.Message.GetBody());
            retryMessage.OriginalMessage.DeliveryTag = EncodeDeliveryTag(messagingEvent.DeliveryTag);
            RetryInformations.Add(retryMessage.OriginalMessage.DeliveryTag, retryMessage.RetryInformations);

            callback(this, retryMessage.OriginalMessage);
        }

        private static string EncodeDeliveryTag(string deliveryTag) //TODO CLEAN !! this is just a fast fix ...
        {
            return $"{deliveryTag}_{Guid.NewGuid().ToString()}";
        }

        private static string DecodeDeliveryTag(string deliveryTag) //TODO CLEAN !! this is just a fast fix
        {
            return deliveryTag.Split('_')[0]; 
        }
    }
}
