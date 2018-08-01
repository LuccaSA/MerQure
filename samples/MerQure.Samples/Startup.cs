using MerQure.RbMQ;
using MerQure.RbMQ.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace MerQure.Samples
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("merqure.rbmq.json", optional: true)
                .AddXmlFile("rabbitMQ.config", optional: true);
              
            Configuration = configBuilder.Build();
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
