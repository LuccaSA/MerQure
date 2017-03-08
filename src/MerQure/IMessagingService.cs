﻿using System.Collections.Generic;

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

        void DeclareQueue(string queueName, int maxPriority);

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
        /// Get the consumer, used to receive messages of a queue
        /// </summary>
        /// <param name="queueName"></param>
        IConsumer GetConsumer(string queueName);
    }
}
