namespace MerQure.RbMQ.Config
{
    public class MerQureConnection
    {
        public const string DefaultConnectionString = "amqp://guest:guest@localhost:5672/";
        private const string AutomaticRecoveryEnabledPropertyName = "automaticRecoveryEnabled";
        private const string TopologyRecoveryEnabledPropertyName = "topologyRecoveryEnabled";

        /// <summary>
        /// ConnectionString Uri to rabbitMQ server
        /// </summary>
        public string ConnectionString { get; set; } = DefaultConnectionString;

        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public bool TopologyRecoveryEnabled { get; set; } = true;
    }
}
