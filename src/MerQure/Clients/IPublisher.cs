using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure
{
    interface IPublisher
    {
        string ExchangeName { get; }

        void Declare();

        void Publish(IMessage message);
    }
}
