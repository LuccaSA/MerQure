namespace MerQure.RbMQ.Events
{
    internal class MessagingEvent : IMessagingEvent
    {
        public IMessage Message { get; set; }

        public string DeliveryTag { get; set; }

        public MessagingEvent(IMessage message, string deliveryTag)
        {
            Message = message;
            DeliveryTag = deliveryTag;
        }
    }
}
