using System;
using System.Collections.Generic;
using Xunit;

namespace MerQure.RbMQ.Tests
{
    public class MessagingServiceTests
    {
        [Fact]
        public void DeclareExchangeFailWithEmptyName()
        {
            var messagingService = new MessagingService(null,null);

            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareExchange(null));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareExchange("    "));
        }

        [Theory]
        [MemberData(nameof(IsQuorumPossibleValues))]
        public void DeclareQueueFailWithEmptyName(bool isQuorum)
        {
            var messagingService = new MessagingService(null, null);

            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareQueue(null, isQuorum: isQuorum));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareQueue("    ", isQuorum: isQuorum));
        }
        public static IEnumerable<object[]> IsQuorumPossibleValues =>
            new List<object[]>
            {
                new object[] { true },
                new object[] { false }
            };

        [Fact]
        public void DeclareBindingFailWithEmptyParameters()
        {
            var messagingService = new MessagingService(null, null);

            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding(null, "queue", "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding("exhange", null, "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding("", "queue", "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding("exhange", "", "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding("exhange", "queue", null));
        }

        [Fact]
        public void CancelBindingFailWithEmptyParameters()
        {
            var messagingService = new MessagingService(null, null);

            Assert.Throws<ArgumentNullException>(() => messagingService.CancelBinding(null, "queue", "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.CancelBinding("exhange", null, "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.CancelBinding("exhange", "queue", null));
        }
    }
}