using MerQure.Tools.Samples.RetryBusExample.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MerQure.Tools.Samples;

public static class Program
{
	static async Task Main(string[] args)
	{
		IServiceCollection services = new ServiceCollection();

		var startup = new Startup();
		startup.ConfigureServices(services);

		var serviceProvider = services.BuildServiceProvider();

		var actionService = serviceProvider.GetService<ActionService>();
		await actionService.ConsumeAsync();

		for (int i = 0; i < 50; i++)
		{
			await actionService.SendNewSampleAsync();
		}

		Console.ReadLine();
	}
}