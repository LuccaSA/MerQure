using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task DeclareExchangeAsync(string exchangeName);

        /// <summary>
        /// Declare an Exchange (if it doesn't exists)
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType">fanout, direct, topic, headers</param>
        Task DeclareExchangeAsync(string exchangeName, string exchangeType);

        /// <summary>
        /// Declare a queue with dead letter policy
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="deadLetterExchange"></param>
        /// <param name="messageTimeToLive"></param>
        /// <param name="deadLetterRoutingKey"></param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        /// <see cref="https://www.rabbitmq.com/dlx.html"/>
        Task<string> DeclareQueueWithDeadLetterPolicyAsync(string queueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey, bool isQuorum);

        Task<string> DeclareQueueAsync(string queueName, byte maxPriority, bool isQuorum);

        /// <summary>
        /// Declare a queue (if it doesn't exists) with specific arguments
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="queueArgs">Queue's arguments</param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        Task<string> DeclareQueueAsync(string queueName, Dictionary<string, object> queueArgs, bool isQuorum);

        /// <summary>
        /// Declare a queue (if it doesn't exists)
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        Task<string> DeclareQueueAsync(string queueName, bool isQuorum);

        /// <summary>
        /// Declare an Binding (if it doesn't exists)
        /// A Binding is the link between an Exchange and a Queue via a RoutingKey
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        Task DeclareBindingAsync(string exchangeName, string queueName, string routingKey);
        /// <summary>
        /// Declare an Binding (if it doesn't exists)
        /// A Binding is the link between an Exchange and a Queue
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        /// <param name="headerBindings"></param>
        Task DeclareBindingAsync(string exchangeName, string queueName, string routingKey, Dictionary<string, object> headerBindings);

        /// <summary>
        /// Cancel an Binding
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routingKey"></param>
        Task CancelBindingAsync(string exchangeName, string queueName, string routingKey);

        /// <summary>
        /// Get the publisher, used to declare exchanges et publish messages on it.
        /// </summary>
        /// <param name="exchangeName"></param>
        Task<IPublisher> GetPublisherAsync(string exchangeName);

        /// <summary>
        /// Get the publisher, used to declare exchanges et publish messages on it.
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name"enablePublisherAcknowledgements">required to use publish with acknowledgements</param>
        Task<IPublisher> GetPublisherAsync(string exchangeName, bool enablePublisherAcknowledgements);

        /// <summary>
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        Task<IConsumer> GetConsumerAsync(string queueName);

        /// <summary>
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">maximum number of messages that the server will deliver, 0 if unlimited</param>
        /// <returns></returns>
        Task<IConsumer> GetConsumerAsync(string queueName, ushort prefetchCount);
    }
}