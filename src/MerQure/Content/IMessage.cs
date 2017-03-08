using MerQure.Content;
using System.Collections.Generic;

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
