using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    interface IMessagingEvent
    {
        IMessage Message { get; set; }

        string DeliveryTag { get; set; }
    }
}
