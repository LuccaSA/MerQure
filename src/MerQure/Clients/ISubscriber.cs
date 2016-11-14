using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    interface ISubscriber
    {
        string QueueName { get; }

        void DeclareSubscribtion(string exchangeName, string routingKey);
    }
}
