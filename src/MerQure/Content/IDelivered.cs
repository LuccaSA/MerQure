using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Messages
{
    public interface IDelivered
    {
        /// <summary>
        /// Identify the message delivered by the queue
        /// </summary>
        string DeliveryTag { get; set; }
    }
}
