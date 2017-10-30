using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Messages
{
    public interface IDelivered
    {
        string DeliveryTag { get; set; }
    }
}
