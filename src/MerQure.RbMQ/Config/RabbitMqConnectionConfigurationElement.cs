using System;

namespace MerQure.RbMQ.Config
{
    public class RabbitMqConnectionConfigurationElement  
    {
        public const string DefaultConnectionString = "amqp://guest:guest@localhost:5672/";
        private const string ConnectionStringPropertyName = "connectionString";
        private const string AutomaticRecoveryEnabledPropertyName = "automaticRecoveryEnabled";
        private const string TopologyRecoveryEnabledPropertyName = "topologyRecoveryEnabled";

        /// <summary>
        /// ConnectionString Uri to rabbitMQ server
        /// </summary>
        public String ConnectionString { get; set; } = DefaultConnectionString;


        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public bool TopologyRecoveryEnabled { get; set; } = true;
    }
}
