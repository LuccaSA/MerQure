using MerQure.Messages;

namespace MerQure.Tools.Messages
{
    public class RetryMessage<T> where T : IDelivered
    {
        public RetryInformations RetryInformations { get; set; }
        public T OriginalMessage { get; set; }
    }
}
