using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    interface IMessage
    {
        string RoutingKey { get; }

        IHeader Header { get; }

        string Body { get; set; }
    }
}
