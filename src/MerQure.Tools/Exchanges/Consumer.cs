using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Exchanges
{
    internal class Consumer<T> where T : IDelivered
    {
        private readonly ConsumerProvider _consumerProvider;
        internal Dictionary<string, MessageTechnicalInformations> TechnicalInformations { get; } = new Dictionary<string, MessageTechnicalInformations>();

        public Consumer(IMessagingService messagingService)
        {
            _consumerProvider = new ConsumerProvider(messagingService);
        }

        public void Consume(Channel channel, EventHandler<T> callback)
        {
            _consumerProvider.Get(channel).Consume((object sender, IMessagingEvent messagingEvent) =>
            {
                EncapsuledMessage<T> deserializedMessage = JsonConvert.DeserializeObject<EncapsuledMessage<T>>(messagingEvent.Message.GetBody());
                deserializedMessage.OriginalMessage.DeliveryTag = messagingEvent.DeliveryTag;
                TechnicalInformations.Add(messagingEvent.DeliveryTag, deserializedMessage.TechnicalInformation);

                callback(this, deserializedMessage.OriginalMessage);
            });
        }

        public void AcknowlegdeDelivredMessage(Channel channel, IDelivered delivredMessage)
        {
            _consumerProvider.Get(channel).AcknowlegdeDeliveredMessage(delivredMessage.DeliveryTag);
        }

        public void RejectDeliveredMessage(Channel channel, IDelivered delivredMessage)
        {
            _consumerProvider.Get(channel).RejectDeliveredMessage(delivredMessage.DeliveryTag);
        }
    }
}
