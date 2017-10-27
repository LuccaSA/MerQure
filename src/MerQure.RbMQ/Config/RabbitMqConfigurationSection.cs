using System;

namespace MerQure.RbMQ.Config
{
    public class RabbitMqConfigurationSection 
    {
        private const string DurablePropertyName = "durable";
        private const string AutoDeleteQueuePropertyName = "autoDeleteQueue";
        private const string DefaultPrefetchCountPropertyName = "defaultPrefetchCount";
        private const string PublisherAcknowledgementsTimeoutPropertyName = "publisherAcknowledgementsTimeoutInMilliseconds";

        private const long PublisherAcknowledgementsTimeoutInMillisecondsDefaultValue = 10000;
        private const ushort DefaultPrefetchCountDefaultValue = 1;
         
        public Boolean Durable { get; set; } = true;
         
        public Boolean AutoDeleteQueue { get; set; } = false;
         
        public ushort DefaultPrefetchCount { get; set; } = DefaultPrefetchCountDefaultValue;
         
        public long PublisherAcknowledgementsTimeoutInMilliseconds { get; set; } = PublisherAcknowledgementsTimeoutInMillisecondsDefaultValue;
         
        public RabbitMqConnectionConfigurationElement Connection { get; set; }
    }
}
