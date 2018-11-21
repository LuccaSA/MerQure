using System;

namespace MerQure.RbMQ.Config
{
    public class MerQureConnection
    {
        public const string DefaultConnectionString = "amqp://guest:guest@localhost:5672/";
        private const string AutomaticRecoveryEnabledPropertyName = "automaticRecoveryEnabled";
        private const string TopologyRecoveryEnabledPropertyName = "topologyRecoveryEnabled";
        private const ushort DefaultRequestedChannelMax = 0;

        /// <summary>
        /// ConnectionString Uri to rabbitMQ server
        /// </summary>
        public String ConnectionString { get; set; } = DefaultConnectionString;

        public String FriendlyName { get; set; }

        public bool AutomaticRecoveryEnabled { get; set; } = true;

        public bool TopologyRecoveryEnabled { get; set; } = true;

        public ushort RequestedChannelMax { get; set; } = DefaultRequestedChannelMax;
    }
}
