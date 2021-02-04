using System;

namespace MerQure.RbMQ.Config
{
    public class MerQureConnection
    {
        public const string DefaultConnectionString = "amqp://guest:guest@localhost:5672/";
        private const ushort DefaultRequestedChannelMax = 0;

        /// <summary>
        /// ConnectionString Uri to rabbitMQ server
        /// Can's be used with ConnectionCluster
        /// </summary>
        public String ConnectionString { get; set; }

        /// <summary>
        /// Connect to a list of endpoint
        /// Can's be used with ConnectionString
        /// </summary>
        public MerQureConnectionCluster ConnectionCluster { get; set; }

        public String FriendlyName { get; set; }

        public bool AutomaticRecoveryEnabled { get; set; } = true;

        public bool TopologyRecoveryEnabled { get; set; } = true;

        public ushort RequestedChannelMax { get; set; } = DefaultRequestedChannelMax;
    }
}
