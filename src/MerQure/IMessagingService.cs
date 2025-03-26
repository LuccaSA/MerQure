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

        /// <summary>
        /// Implementation of name transformation between proposedQueueName and queue name effectively used made by each DeclareQueueXxxx method
        /// </summary>
        /// <param name="proposedQueueName"></param>
        /// <param name="isQuorum"></param>
        /// <returns>Effective queue name</returns>
        string QueueNameTransformationUsedByDeclareQueue(string proposedQueueName, bool isQuorum);
        
        /// <summary>
        /// Declare a queue with dead letter policy
        /// </summary>
        /// <param name="proposedQueueName">Proposed queue name (will be transformetransformed by <see cref="QueueNameTransformationUsedByDeclareQueue"/>)</param>
        /// <param name="deadLetterExchange"></param>
        /// <param name="messageTimeToLive"></param>
        /// <param name="deadLetterRoutingKey"></param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        /// <see cref="https://www.rabbitmq.com/dlx.html"/>
        /// <returns>Effective queue name used by created queue</returns>
        string DeclareQueueWithDeadLetterPolicy(string proposedQueueName, string deadLetterExchange, int messageTimeToLive, string deadLetterRoutingKey, bool isQuorum);

        string DeclareQueue(string proposedQueueName, byte maxPriority, bool isQuorum);

        /// <summary>
        /// Declare a queue (if it doesn't exists) with specific arguments
        /// </summary>
        /// <param name="proposedQueueName">Proposed queue name (transformed by <see cref="QueueNameTransformationUsedByDeclareQueue"/>)</param>
        /// <param name="queueArgs">Queue's arguments</param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        /// <returns>Effective queue name used by created queue</returns>
        string DeclareQueue(string proposedQueueName, Dictionary<string, object> queueArgs, bool isQuorum);

        /// <summary>
        /// Declare a queue (if it doesn't exists)
        /// </summary>
        /// <param name="proposedQueueName">Proposed queue name (transformed by <see cref="QueueNameTransformationUsedByDeclareQueue"/>)</param>
        /// <param name="isQuorum">Whether created queue should be quorum (when true, quorum arguments will be added to arguments used for queue creation)</param>
        /// <returns>Effective queue name used by created queue</returns>
        string DeclareQueue(string proposedQueueName, bool isQuorum);

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
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        IConsumer GetConsumer(string queueName);

        /// <summary>
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="prefetchCount">maximum number of messages that the server will deliver, 0 if unlimited</param>
        /// <returns></returns>
        IConsumer GetConsumer(string queueName, ushort prefetchCount);
    }
}
