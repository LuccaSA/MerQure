using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using MerQure.Tools.Exceptions;
using MerQure.Tools.Messages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MerQure.Tools.Tests.Buses
{
    public class PublisherTests : IDisposable
    {
        private Mock<IPublisher> _mockMerQurePublisher;
        private Mock<IMessagingService> _mockMessagingService;

        private Publisher<TestMessage> _publisher;
        private RetryStrategyConfiguration _retryStrategy;

        public PublisherTests()
        {
            _mockMerQurePublisher = new Mock<IPublisher>();
            _mockMessagingService = new Mock<IMessagingService>();

            _retryStrategy = new RetryStrategyConfiguration()
            {
                BusName = "testBus",
                Channels = new List<Channel>()
                {
                    new Channel("testChannel")
                },
                DelaysInMsBetweenEachRetry = new List<int>()
                {
                    1000,
                    2000
                },
                DeliveryDelayInMilliseconds = 0,
                MessageIsGoingIntoErrorBusAfterAllRepeat = false
            };

            _publisher = new Publisher<TestMessage>(_mockMessagingService.Object, _retryStrategy);
        }

        public void Dispose()
        {
            _mockMerQurePublisher = null;
            _mockMessagingService = null;
            _publisher = null;
            _retryStrategy = null;
        }

        [Fact]
        public void Publisher_Publish_MessageShouldBePublishedOnRightBusAndChannel()
        {
            //Arrange
            Channel channel = new Channel("testChannel");
            TestMessage message = TestMessage.GetFilledTestMessage();

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), true)).Returns(_mockMerQurePublisher.Object);
            _mockMerQurePublisher.Setup(p => p.PublishWithAcknowledgement(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            //Act
            _publisher.Publish(channel, message, false);

            //Assert
            _mockMessagingService.Verify(p => p.GetPublisher(It.Is<string>(busName => busName.Contains(_retryStrategy.BusName)), true));
            _mockMerQurePublisher.Verify(p => p.PublishWithAcknowledgement(It.Is<string>(channelName => channelName.Contains(channel.Value)), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Publisher_Publish_ShouldThrow_WhenPublishFailed()
        {
            //Arrange
            Channel channel = new Channel("testChannel");
            TestMessage message = TestMessage.GetFilledTestMessage();

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), true)).Returns(_mockMerQurePublisher.Object);
            _mockMerQurePublisher.Setup(p => p.PublishWithAcknowledgement(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            //Act && Assert
            Assert.Throws<MerqureToolsException>(() => _publisher.Publish(channel, message, false));
        }

        [Fact]
        public void Publisher_PublishWithTransaction_MessageShouldBePublishedOnRightBusAndChannel()
        {
            //Arrange
            Channel channel = new Channel("testChannel");
            var messages = new List<TestMessage>
            {
                TestMessage.GetFilledTestMessage()
            };

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), false)).Returns(_mockMerQurePublisher.Object);

            //Act
            _publisher.PublishWithTransaction(channel, messages, false);

            //Assert
            _mockMessagingService.Verify(p => p.GetPublisher(It.Is<string>(busName => busName.Contains(_retryStrategy.BusName)), false));
            _mockMerQurePublisher.Verify(p => p.PublishWithTransaction(It.Is<string>(channelName => channelName.Contains(channel.Value)), It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public void Publisher_PublishWithTransaction_ShouldThrow_WhenPublishWithTransactionFailed()
        {
            //Arrange
            Channel channel = new Channel("testChannel");
            var messages = new List<TestMessage>
            {
                TestMessage.GetFilledTestMessage()
            };

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), false)).Returns(_mockMerQurePublisher.Object);
            _mockMerQurePublisher.Setup(p => p.PublishWithTransaction(It.IsAny<string>(), It.IsAny<List<string>>())).Throws<Exception>();

            //Act && Assert
            Assert.Throws<MerqureToolsException>(() => _publisher.PublishWithTransaction(channel, messages, false));
        }

        [Fact]
        public void Publisher_PublishOnRetryExchange_MessageShouldBePublishedOnRightBusAndChannel()
        {
            Channel channel = new Channel("testChannel");
            TestMessage message = TestMessage.GetFilledTestMessage();

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), true)).Returns(_mockMerQurePublisher.Object);
            _mockMerQurePublisher.Setup(p => p.PublishWithAcknowledgement(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _publisher.PublishOnRetryExchange(channel, message, new RetryInformations
            {
                NumberOfRetry = 0
            });

            _mockMessagingService.Verify(p => p.GetPublisher(It.Is<string>(busName => busName.Contains(_retryStrategy.BusName)), true));
            _mockMerQurePublisher.Verify(p => p.PublishWithAcknowledgement(It.Is<string>(channelName => channelName.Contains(channel.Value)), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Publisher_PublishOnRetryExchange_QueueNameShouldContainsFirstDelay_WhenItsTheFirstRetry()
        {
            Channel channel = new Channel("testChannel");
            TestMessage message = TestMessage.GetFilledTestMessage();

            _mockMessagingService.Setup(m => m.GetPublisher(It.IsAny<string>(), true)).Returns(_mockMerQurePublisher.Object);
            _mockMerQurePublisher.Setup(p => p.PublishWithAcknowledgement(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _publisher.PublishOnRetryExchange(channel, message, new RetryInformations
            {
                NumberOfRetry = 0
            });

            _mockMessagingService.Verify(p => p.GetPublisher(It.Is<string>(busName => busName.Contains(_retryStrategy.BusName)), true));
            _mockMerQurePublisher.Verify(p => p.PublishWithAcknowledgement(It.Is<string>(channelName => channelName.Contains(_retryStrategy.DelaysInMsBetweenEachRetry.First().ToString())), 
                                                                           It.IsAny<string>()), Times.Once);
        }
    }
}
