namespace MerQure.RbMQ.Config
{
    public class MerQureConnectionCluster
    {
        /// <summary>
        /// List of rabbitmq nodes.
        /// Format : "hostname:port"
        /// </summary>
        public string[] Nodes { get; set; }

        public bool Ssl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }

    }
}
