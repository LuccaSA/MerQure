using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Content
{
    public interface IAmqpIdentity
    {
        string DeliveryTag { get; set; }
    }
}
