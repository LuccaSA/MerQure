using MerQure.Tools.Samples.RetryBusExample.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MerQure.Tools.Samples
{
	public class Program
	{
		static void Main(string[] args)
		{
			IServiceCollection services = new ServiceCollection();

			var startup = new Startup();
			startup.ConfigureServices(services);

			var serviceProvider = services.BuildServiceProvider();

			var actionService = serviceProvider.GetService<ActionService>();
			actionService.Consume();

			for (int i = 0; i < 50; i++)
			{
				actionService.SendNewSample();
			}

			Console.ReadLine();
		}
	}
}
