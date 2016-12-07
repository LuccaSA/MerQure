using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Config
{
    public static class RabbitMqConfiguration
    {
        public static RabbitMqConfigurationSection GetConfig()
        {
            var section = System.Configuration.ConfigurationManager.GetSection("rabbitMQ") ?? new RabbitMqConfigurationSection();
            return (RabbitMqConfigurationSection)section;
        }
    }
}
