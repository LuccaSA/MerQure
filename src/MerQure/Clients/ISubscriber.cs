namespace MerQure
{
    /// <summary>
    /// Subscriber interface. Used to declare a queues and its subscribtions on exchanges
    /// </summary>
    public interface ISubscriber
    {
        string QueueName { get; }

        /// <summary>
        /// Bind a queue to an exchange in topic mode
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey">routing key defining the subscribtion</param>
        void DeclareSubscribtion(string exchangeName, string routingKey);
    }
}
