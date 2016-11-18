namespace MerQure
{
    public interface IMessagingEvent
    {
        IMessage Message { get; set; }

        /// <summary>
        /// Identify the message delivered by the queue
        /// </summary>
        string DeliveryTag { get; set; }
    }
}
