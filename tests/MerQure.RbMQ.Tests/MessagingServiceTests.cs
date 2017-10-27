using System;
using Xunit;
using MerQure.RbMQ;

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

        [Fact]
        public void DeclareQueueFailWithEmptyName()
        {
            var messagingService = new MessagingService(null, null);

            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareQueue(null));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareQueue("    "));
        }

        [Fact]
        public void DeclareBindingFailWithEmptyParameters()
        {
            var messagingService = new MessagingService(null, null);

            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding(null, "queue", "routing"));
            Assert.Throws<ArgumentNullException>(() => messagingService.DeclareBinding("exhange", null, "routing"));
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