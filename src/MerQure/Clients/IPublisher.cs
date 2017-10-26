using System;
using System.Collections.Generic;

namespace MerQure
{
    /// <summary>
    /// Publisher interface. Used to publish messages on an exchange.
    /// </summary>
    public interface IPublisher : IDisposable
    {
        /// <summary>
        /// Exchange where publish messages
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// Publishes a message
        /// </summary>
        /// <param name="message"></param>
        void Publish(IMessage message);

        /// <summary>
        /// Publishes a message with broker confirmation.
        /// Waits until all messages published since the last call have been confirmed. Default timeout is 10000 milliseconds. 
        /// Set the publisherAcknowledgementsTimeoutInMilliseconds attribute in RabbitMQ.config to change it.
        /// if PublisherAcknowledgements is not activated an exception is throws
        /// </summary>
        /// <param name="message">message</param>
        /// <see cref="http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/"/>
        bool PublishWithAcknowledgement(IMessage message);

        /// <summary>
        /// Publishes a message with broker confirmation.
        /// Waits until all messages published since the last call have been confirmed. Default timeout is 10000 milliseconds. 
        /// Set the publisherAcknowledgementsTimeoutInMilliseconds attribute in RabbitMQ.config to change it.
        /// if PublisherAcknowledgements is not activated an exception is throws
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="message">message</param>
        /// <see cref="http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/"/>
        bool PublishWithAcknowledgement(string queueName, string message);

        /// <summary>
        /// Publishes multiple messages.
        /// Transaction is activated, if there is any error, all messages will be rollbacked.
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="messages">messages</param>
        void PublishWithTransaction(string queueName, IEnumerable<string> messages);
    }
}
