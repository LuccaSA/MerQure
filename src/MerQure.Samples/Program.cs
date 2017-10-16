using MerQure.Samples.RetryExchange.Domain;
using MerQure.Samples.RetryExchange.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.Samples
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Running sample 1: simple");
            SimpleExample.Run();

            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Running sample 2: stop");
            StopExample.Run();

            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Running sample 3: deadletter");
            DeadLetterExample.Run();

            Console.WriteLine("Running sample 4 : retry exchange");
            ISomethingService somethingService = new SomethingService();
            var actionService = new SampleService(somethingService);
            actionService.Consume();
            for (int i = 0; i < 50; i++)
            {
                actionService.SendNewSomething();
            }

            Console.ReadLine();
        }
    }
}
