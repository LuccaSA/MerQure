
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples.RetryBusExample.Domain
{
    public interface ISampleService
    {
        void Send(Sample sample);
        void Consume(EventHandler<Sample> onSampleReceived);
        void Acknowlegde(Sample sample);
        void RetryLater(Sample sample);
        void SendOnError(Sample sample); 
    }
}
