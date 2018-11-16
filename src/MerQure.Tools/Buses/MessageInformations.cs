namespace MerQure.Tools.Buses
{
    public class MessageInformations
    {
        /// <summary>
        /// Is true if the message is on error bus.
        /// Can be true only if <see cref="RetryStrategyConfiguration.MessageIsGoingIntoErrorBusAfterAllRepeat"/> is true.
        /// </summary>
        public bool IsOnErrorBus { get; set; }
    }
}