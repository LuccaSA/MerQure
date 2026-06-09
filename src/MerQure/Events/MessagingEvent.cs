using MerQure.Messages;
using RabbitMQ.Client.Events;

namespace MerQure
{
    public class MessagingEvent : AsyncEventArgs, IMessagingEvent
    {
        public IMessage Message { get; set; }

        public string DeliveryTag { get; set; }

        public MessagingEvent(IMessage Message, string DeliveryTag)
        {
            this.Message = Message;
            this.DeliveryTag = DeliveryTag;
        }
    }
}
