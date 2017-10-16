using MerQure.Configuration;
using MerQure.Content;
using System.Collections.Generic;

namespace MerQure
{
    /// <summary>
    /// This service expose all clients necessary to used basic functionnalities of a Message Broker 
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// Declare an Exchange (if it doesn't exists)
        /// </summary>
        /// <param name="exchangeName"></param>
        void DeclareExchange(string exchangeName);

        /// <summary>
        /// Declare an Exchange (if it doesn't exists)
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType">fanout, direct, topic, headers</param>
        void DeclareExchange(string exchangeName, string exchangeType);

        void DeclareQueue(string queueName, byte maxPriority);

        /// <summary>
        /// Declare a queue iwth dead letter policy
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="deadLetterExchange"></param>
        /// <param name="messageTimeToLive"></param>
        /// <param name="deadLetterRoutingKey"></param>
        /// <see cref="https://www.rabbitmq.com/dlx.html"/>
        void DeclareQueueWithDeadLetterPolicy(string queueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey);

        /// <summary>
        /// Declare a queue (if it doesn't exists) with specifi arguments
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="queueArgs">Queue's arguments</param>
        void DeclareQueue(string queueName, Dictionary<string, object> queueArgs);

        /// <summary>
        /// Declare a queue (if it doesn't exists)
        /// </summary>
        /// <param name="queueName"></param>
        void DeclareQueue(string queueName);

        /// <summary>
        /// Declare an Binding (if it doesn't exists)
        /// A Binding is the link between an Exchange and a Queue via a RoutingKey
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        void DeclareBinding(string exchangeName, string queueName, string routingKey);
        /// <summary>
        /// Declare an Binding (if it doesn't exists)
        /// A Binding is the link between an Exchange and a Queue
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        /// <param name="headerBindings"></param>
        void DeclareBinding(string exchangeName, string queueName, string routingKey, Dictionary<string, object> headerBindings);

        /// <summary>
        /// Cancel an Binding
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        void CancelBinding(string exchangeName, string queueName, string routingKey);

        /// <summary>
        /// Get the publisher, used to declare exchanges et publish messages on it.
        /// </summary>
        /// <param name="exchangeName"></param>
        IPublisher GetPublisher(string exchangeName);

        /// <summary>
        /// Get the publisher, used to declare exchanges et publish messages on it.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name"enablePublisherAcknowledgements">required to use publish with acknowledgements</param>
        IPublisher GetPublisher(string exchangeName, bool enablePublisherAcknowledgements);

        /// <summary>
        /// Get the consumer or create if it is not existing, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        IConsumer GetOrCreateConsumer(string queueName);

        /// <summary>
        /// Get the consumer or create if it is not existing, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">maximum number of messages that the server will deliver, 0 if unlimited</param>
        /// <returns></returns>
        IConsumer GetOrCreateConsumer(string queueName, ushort prefetchCount);

        /// <summary>
        /// Create a new consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        IConsumer CreateConsumer(string queueName);

        /// <summary>
        /// Create a new consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">maximum number of messages that the server will deliver, 0 if unlimited</param>
        IConsumer CreateConsumer(string queueName, ushort prefetchCount);
    }
}
