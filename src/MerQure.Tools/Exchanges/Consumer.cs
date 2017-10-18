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
    internal class Consumer<T> where T : IAMQPIdentity
    {
        private readonly ConsumerProvider _consumerProvider;
        internal Dictionary<string, MessageTechnicalInformations> _technicalInformations;

        public Consumer(IMessagingService messagingService)
        {
            _consumerProvider = new ConsumerProvider(messagingService);
            _technicalInformations = new Dictionary<string, MessageTechnicalInformations>();
        }
        public void Consume(Channel channel, EventHandler<T> callback)
        {
            _consumerProvider.Get(channel).Consume((object sender, IMessagingEvent messagingEvent) =>
            {
                EncapsuledMessage<T> deserializedMessage = JsonConvert.DeserializeObject<EncapsuledMessage<T>>(messagingEvent.Message.GetBody());
                deserializedMessage.OriginalMessage.DeliveryTag = messagingEvent.DeliveryTag;
                _technicalInformations.Add(messagingEvent.DeliveryTag, deserializedMessage.TechnicalInformation);

                callback(this, deserializedMessage.OriginalMessage);
            });
        }

        public void AcknowlegdeDelivredMessage(Channel channel, IAMQPIdentity delivredMessage)
        {
            _consumerProvider.Get(channel).AcknowlegdeDeliveredMessage(delivredMessage.DeliveryTag);
        }

        public void RejectDeliveredMessage(Channel channel, IAMQPIdentity delivredMessage)
        {
            _consumerProvider.Get(channel).RejectDeliveredMessage(delivredMessage.DeliveryTag);
        }
    }
}
