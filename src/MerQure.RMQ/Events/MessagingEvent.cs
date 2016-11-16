namespace MerQure.RMQ.Events
{
    class MessagingEvent : IMessagingEvent
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
