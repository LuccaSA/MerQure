using RabbitMQ.Client;
using System.Collections.Generic;

namespace MerQure.RbMQ.Clients
{
    class Subscriber : RabbitMqClient, ISubscriber
    {
        private bool queueDeclared = false;

        public string QueueName { get; private set; }
        public bool Durable { get; private set; }
        public bool AutoDeleteQueue { get; private set; }

        /// <summary>
        /// Declare a  RabbitMQ Subscriber
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueName"></param>
        /// <param name="durable"></param>
        /// <param name="autoDeleteQueue"></param>
        public Subscriber(IModel channel, string queueName, bool durable, bool autoDeleteQueue)
            : base(channel)
        {
            this.QueueName = queueName.ToLowerInvariant();
            this.Durable = durable;
            this.AutoDeleteQueue = autoDeleteQueue;
        }

        public void DeclareSubscription(string exchangeName, string routingKey)
        {
            if (!queueDeclared)
            {
                var queueArgs = new Dictionary<string, object>();

                this.Channel.QueueDeclare(this.QueueName, this.Durable, false, this.AutoDeleteQueue, queueArgs);
                queueDeclared = true;
            }

            this.Channel.QueueBind(this.QueueName, exchangeName.ToLowerInvariant(), routingKey.ToLowerInvariant());
        }
    }
}
