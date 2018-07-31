using Microsoft.Extensions.DependencyInjection;
using System;

namespace MerQure.Samples
{
    static class Program
    {
        static void Main()
        {
            IServiceCollection services = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Running sample 1: simple");
            var simpleExample = serviceProvider.GetService<SimpleExample>();
            simpleExample.Run();

            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Running sample 2: stop");
            var stopExample = serviceProvider.GetService<StopExample>();
            stopExample.Run();

            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Running sample 3: deadletter");
            var deadLetterExample = serviceProvider.GetService<DeadLetterExample>();
            deadLetterExample.Run();
        }
    }
}
