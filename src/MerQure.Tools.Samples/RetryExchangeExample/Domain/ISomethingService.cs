
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryExchangeExample.Domain
{
    public interface ISomethingService
    {
        void Send(Something thing);
        void Consume(EventHandler<Something> onSomethingReceived);
        void Acknowlegde(Something something);
        void RetryLater(Something something);
        void SendOnError(Something something); 
    }
}
