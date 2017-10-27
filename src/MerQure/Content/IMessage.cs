using MerQure.Content;

namespace MerQure
{
    public interface IMessage
    {
        string GetRoutingKey();

        IHeader GetHeader();

        string GetBody();

        byte? GetPriority();
    }
}
