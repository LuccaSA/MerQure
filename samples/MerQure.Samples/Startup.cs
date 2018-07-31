using MerQure.RbMQ;
using MerQure.RbMQ.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;

namespace MerQure.Samples
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
            var dir = Directory.GetCurrentDirectory();

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(dir)
                .AddJsonFile("merqure.rbmq.json", optional: true)
                .AddXmlFile("rabbitMQ.config", optional: true);
              
            Configuration = configBuilder.Build();
            configBuilder.AddConfiguration(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MerQureConfiguration>(Configuration);

            services.AddSingleton<SharedConnection>();
            services.AddScoped<IMessagingService, MessagingService>();

            services.AddScoped<SimpleExample>();
            services.AddScoped<StopExample>();
            services.AddScoped<DeadLetterExample>();
        }
    }
}
