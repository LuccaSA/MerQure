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
        /// Publishes a message with borker confirmation.
        /// Waits until all messages published since the last call have
        /// throws an exception when called on a non-Confirm channel
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="timeout">how long to wait (at most) before returning</param>
        /// <returns>True if no nacks were received within the timeout, otherwise false</returns>
        bool Publish(IMessage message, TimeSpan timeout);
    }
}
