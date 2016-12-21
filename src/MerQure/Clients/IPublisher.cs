namespace MerQure
{
    /// <summary>
    /// Publisher interface. Used to publish messages on an exchange.
    /// </summary>
    public interface IPublisher
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
    }
}
