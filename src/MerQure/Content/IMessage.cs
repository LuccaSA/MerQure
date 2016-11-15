using MerQure.Content;

namespace MerQure
{
    public interface IMessage
    {
        string RoutingKey { get; }

        IHeader Header { get; }

        string Body { get; set; }
    }
}
