using MerQure.RbMQ;
using MerQure.Tools.Samples.RetryExchangeExample.Domain;
using MerQure.Tools.Samples.RetryExchangeExample.RetryExchangeExample.Infra;
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
            ISomethingService somethingService = new SomethingService(retryBusService);

            ActionService actionService = new ActionService(somethingService);
            actionService.Consume();

            for (int i = 0; i < 50; i++)
            {
                actionService.SendNewSomething();
            }

            Console.ReadLine();
        }
    }
}
