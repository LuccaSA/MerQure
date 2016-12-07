using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Config
{
    public class RabbitMqConfigurationSection : ConfigurationSection
    {
        private const string DurablePropertyName = "durable";
        private const string AutoDeleteQueuePropertyName = "autoDeleteQueue";
        
        [ConfigurationProperty(DurablePropertyName, DefaultValue = true, IsRequired = false)]
        public Boolean Durable
        {
            get
            {
                return (Boolean)this[DurablePropertyName];
            }
            set
            {
                this[DurablePropertyName] = value;
            }
        }

        [ConfigurationProperty(AutoDeleteQueuePropertyName, DefaultValue = false, IsRequired = false)]
        public Boolean AutoDeleteQueue
        {
            get
            {
                return (Boolean)this[AutoDeleteQueuePropertyName];
            }
            set
            {
                this[AutoDeleteQueuePropertyName] = value;
            }
        }

        [ConfigurationProperty("connection")]
        public RabbitMqConnectionConfigurationElement Connection
        {
            get { return (RabbitMqConnectionConfigurationElement)this["connection"]; }
            set { this["connection"] = value; }
        }
    }
}
