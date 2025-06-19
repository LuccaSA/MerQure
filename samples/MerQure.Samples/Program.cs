using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MerQure.Samples
{
    static class Program
    {
        static async Task Main()
        {
            IServiceCollection services = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Running sample 1: simple");
            var simpleExample = serviceProvider.GetService<SimpleExample>();
            await simpleExample.RunAsync();

            Thread.Sleep(500);
            Console.WriteLine("Running sample 2: stop");
            var stopExample = serviceProvider.GetService<StopExample>();
            await stopExample.RunAsync();

            Thread.Sleep(500);
            Console.WriteLine("Running sample 3: deadletter");
            var deadLetterExample = serviceProvider.GetService<DeadLetterExample>();
            await deadLetterExample.RunAsync();
        }
    }
}