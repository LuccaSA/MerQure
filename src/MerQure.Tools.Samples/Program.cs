using MerQure.RbMQ;
using MerQure.Tools.Samples.RetryBusExample.Domain;
using MerQure.Tools.Samples.RetryBusExample.RetryExchangeExample.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Retry exchange sample

            IMessagingService messagingService = new MessagingService();
            var retryBusService = new RetryBusService(messagingService);
            ISampleService sampleService = new SampleService(retryBusService);

            ActionService actionService = new ActionService(sampleService);
            actionService.Consume();

            for (int i = 0; i < 50; i++)
            {
                actionService.SendNewSample();
            }

            Console.ReadLine();
        }
    }
}
