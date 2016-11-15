namespace MerQure
{
    public interface IPublisher
    {
        string ExchangeName { get; }

        void Declare();

        void Publish(IMessage message);
    }
}
