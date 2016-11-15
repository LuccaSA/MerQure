namespace MerQure
{
    public interface IMessagingEvent
    {
        IMessage Message { get; set; }

        string DeliveryTag { get; set; }
    }
}
