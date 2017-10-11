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
        /// Waits until all messages published since the last call have been confirmed. Default timeout is 60000MS. 
        /// Set the publisherAcknowledgementsTimeoutInMS attribute in RabbitMQ.config to change it.
        /// </summary>
        /// <param name="message">message</param>
        /// <see>http://www.rabbitmq.com/blog/2011/02/10/introducing-publisher-confirms/</see>
        bool PublishWithAcknowledgement(IMessage message);
    }
}
