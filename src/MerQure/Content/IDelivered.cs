namespace MerQure.Messages
{
    public interface IDelivered
    {
        /// <summary>
        /// Identify the message delivered by the queue
        /// </summary>
        string DeliveryTag { get; set; }
    }
}
