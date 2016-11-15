namespace MerQure
{
    public interface ISubscriber
    {
        string QueueName { get; }

        void DeclareSubscribtion(string exchangeName, string routingKey);
    }
}
