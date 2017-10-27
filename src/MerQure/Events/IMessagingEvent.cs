using MerQure.Messages;

namespace MerQure
{
    public interface IMessagingEvent : IDelivered
    {
        IMessage Message { get; set; }
    }
}
