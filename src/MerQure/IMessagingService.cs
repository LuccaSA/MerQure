using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    // TODO rename ?
    interface IMessagingService
    {
        IPublisher GetPublisher(string exchangeName);

        ISubscriber GetSubscriber(string queueName);

        IConsumer GetConsumer(string queueName, bool noAck);
    }
}
