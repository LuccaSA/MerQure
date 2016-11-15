namespace MerQure
{
    public interface IMessagingService
    {
        IPublisher GetPublisher(string exchangeName);

        ISubscriber GetSubscriber(string queueName);

        IConsumer GetConsumer(string queueName);
    }
}
