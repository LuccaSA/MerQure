using System;

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
        /// </summary>
        /// <param name="message">message</param>
        /// <see cref="http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/"/>
        bool PublishWithAcknowledgement(IMessage message);

        /// <summary>
        /// Publishes a message with broker confirmation.
        /// Waits until all messages published since the last call have been confirmed. Default timeout is 10000 milliseconds. 
        /// Set the publisherAcknowledgementsTimeoutInMilliseconds attribute in RabbitMQ.config to change it.
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="message">message</param>
        /// <see cref="http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/"/>
        bool PublishWithAcknowledgement(string queueName, string message);
    }
}
