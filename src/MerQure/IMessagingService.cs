namespace MerQure
{
    /// <summary>
    /// This service expose all clients necessary to used basic functionnalities of a Message Broker 
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// Get the publisher, used to declare exchanges et publish messages on it.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <returns></returns>
        IPublisher GetPublisher(string exchangeName);

        /// <summary>
        /// Get the subscriber, used to declare queues and their subscriptions on exchanges
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        ISubscriber GetSubscriber(string queueName);

        /// <summary>
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IConsumer GetConsumer(string queueName);
    }
}
