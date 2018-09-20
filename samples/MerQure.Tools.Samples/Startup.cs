using MerQure.RbMQ;
using MerQure.RbMQ.Config;
using MerQure.Tools.Samples.RetryBusExample.Domain;
using MerQure.Tools.Samples.RetryBusExample.RetryExchangeExample.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace MerQure.Tools.Samples
{
	public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
			var p = Directory.GetCurrentDirectory();

			var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("rabbitMQ.json");
              
            Configuration = configBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<MerQureConfiguration>(Configuration.GetSection("rabbitMQ"));

			services.AddSingleton<SharedConnection>();

            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<IRetryBusService, RetryBusService>();
            services.AddScoped<ISampleService, SampleService>();

			services.AddScoped<ActionService>();
		}
	}
}
