using MerQure.Messages;
using System;

namespace MerQure.Tools.Tests
{
    public class TestMessage : IDelivered
    {
        public string DeliveryTag { get; set; }
        public Guid Guid { get; set; }

        public static TestMessage GetFilledTestMessage()
        {
            return new TestMessage
            {
                DeliveryTag = new Random().Next().ToString(),
                Guid = Guid.NewGuid()
            };
        }
    }
}
