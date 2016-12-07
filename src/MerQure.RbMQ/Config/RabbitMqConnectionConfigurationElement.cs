using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerQure.RbMQ.Config
{
    public class RabbitMqConnectionConfigurationElement : ConfigurationElement
    {
        public const string DefaultConnectionString = "amqp://guest:guest@localhost:5672/";
        private const string ConnectionStringPropertyName = "connectionString";
        private const string AutomaticRecoveryEnabledPropertyName = "automaticRecoveryEnabled";
        private const string TopologyRecoveryEnabledPropertyName = "topologyRecoveryEnabled";

        /// <summary>
        /// ConnectionString Uri to rabbitMQ server
        /// </summary>
        /// <see cref="https://www.rabbitmq.com/uri-spec.html"/>
        [ConfigurationProperty(ConnectionStringPropertyName, DefaultValue = DefaultConnectionString, IsRequired = true)]
        public String ConnectionString
        {
            get
            {
                return (String)this[ConnectionStringPropertyName];
            }
            set
            {
                this[ConnectionStringPropertyName] = value;
            }
        }

        [ConfigurationProperty(AutomaticRecoveryEnabledPropertyName, DefaultValue = true, IsRequired = false)]
        public Boolean AutomaticRecoveryEnabled
        {
            get
            {
                return (Boolean)this[AutomaticRecoveryEnabledPropertyName];
            }
            set
            {
                this[AutomaticRecoveryEnabledPropertyName] = value;
            }
        }

        [ConfigurationProperty(TopologyRecoveryEnabledPropertyName, DefaultValue = true, IsRequired = false)]
        public Boolean TopologyRecoveryEnabled
        {
            get
            {
                return (Boolean)this[TopologyRecoveryEnabledPropertyName];
            }
            set
            {
                this[TopologyRecoveryEnabledPropertyName] = value;
            }
        }
    }
}
