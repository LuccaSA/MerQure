using MerQure.Tools.Configurations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MerQure.Tools.Tests
{
    public class RetryBusServiceTests : IDisposable
    {
        private Mock<IMessagingService> _mockMessagingService;

        private RetryBusService _retryBusService;

        private readonly List<string> _channelNames = new List<string>()
        {
            "testChannel",
            "testChannel2"
        };

        public RetryBusServiceTests()
        {
            _mockMessagingService = new Mock<IMessagingService>();

            _retryBusService = new RetryBusService(_mockMessagingService.Object); 
        }

        public void Dispose()
        {
            _mockMessagingService = null;
            _retryBusService = null;
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldThrow_WhenEmptyChannels()
        {
            //Arrange
            var retryStrategy = GetMinimalValidConfiguration();
            retryStrategy.Channels = new List<Channel>();

            //Act && Assert
            Assert.Throws<ArgumentNullException>(() => _retryBusService.CreateNewBus<TestMessage>(retryStrategy));
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldThrow_WhenNullChannels()
        {
            //Arrange
            var retryStrategy = GetMinimalValidConfiguration();
            retryStrategy.Channels = null;

            //Act && Assert
            Assert.Throws<ArgumentNullException>(() => _retryBusService.CreateNewBus<TestMessage>(retryStrategy));
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareTopicExchangeWithBusName()
        {
            //Arrange
            var retryStrategy = GetMinimalValidConfiguration();
 
            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
            _mockMessagingService.Verify(m => m.DeclareExchange(retryStrategy.BusName, Constants.ExchangeTypeTopic));
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareQueueForeachChannel()
        {
            //Arrange
            var retryStrategy = GetMinimalValidConfiguration();

            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
            _mockMessagingService.Verify(m => m.DeclareQueue(_channelNames[0]));
            _mockMessagingService.Verify(m => m.DeclareQueue(_channelNames[1]));
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareBindingForeachChannel()
        {
            //Arrange
            var retryStrategy = GetMinimalValidConfiguration();

            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
            _mockMessagingService.Verify(m => m.DeclareBinding(retryStrategy.BusName, _channelNames[0], It.IsAny<string>()));
            _mockMessagingService.Verify(m => m.DeclareBinding(retryStrategy.BusName, _channelNames[1], It.IsAny<string>()));
        }


        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareDirectRetryExchangeWithBusName_WhenRetryStrategyContainsDelays()
        {
            //Arrange
            var retryStrategy = GetValidConfigurationWithDelay();

            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
            _mockMessagingService.Verify(m => m.DeclareExchange(It.Is<string>(exchangeName => exchangeName.Contains(retryStrategy.BusName) && exchangeName.Contains(RetryStrategyConfiguration.RetryExchangeSuffix)), 
                                                                Constants.ExchangeTypeDirect));
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareDealLetterQueueForeachChannelAndDelay_WhenRetryStrategyContainsDelays()
        {
            //Arrange
            var retryStrategy = GetValidConfigurationWithDelay();

            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
            foreach(int delay in retryStrategy.DelaysInMsBetweenEachRetry)
            {
                _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicy(It.Is<string>(queueName => queueName.Contains(_channelNames[0])),
                                                                                     retryStrategy.BusName,
                                                                                     delay,
                                                                                     null));
                _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicy(It.Is<string>(queueName => queueName.Contains(_channelNames[1])),
                                                                                     retryStrategy.BusName,
                                                                                     delay,
                                                                                     null));
            }
        }

        [Fact]
        public void RetryBusService_CreateNewBus_ShouldDeclareDealLetterQueueForeachChannelAndDelay_WhenRetryStrategyContainsPublishingDelay()
        {
            //Arrange
            var retryStrategy = GetValidConfigurationWithPublishingDelay();

            //Act
            _retryBusService.CreateNewBus<TestMessage>(retryStrategy);

            //Assert 
           _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicy(It.Is<string>(queueName => queueName.Contains(_channelNames[0])),
                                                                                     retryStrategy.BusName,
                                                                                     retryStrategy.DeliveryDelayInMilliseconds,
                                                                                     null));
        }

        private RetryStrategyConfiguration GetMinimalValidConfiguration()
        {
            return new RetryStrategyConfiguration()
            {
                BusName = "busName",
                Channels = new List<Channel>
                {
                    new Channel(_channelNames[0]),
                    new Channel(_channelNames[1])
                },
                DelaysInMsBetweenEachRetry = new List<int>()
            };
        }

        private RetryStrategyConfiguration GetValidConfigurationWithPublishingDelay()
        {
            RetryStrategyConfiguration configuration = GetMinimalValidConfiguration();
            configuration.DeliveryDelayInMilliseconds = 1000;
            return configuration;
        }

        private RetryStrategyConfiguration GetValidConfigurationWithDelay()
        {
            RetryStrategyConfiguration configuration = GetMinimalValidConfiguration();
            configuration.DelaysInMsBetweenEachRetry = new List<int>
            {
                1000,
                2000
            };
            return configuration;
        }
    }
}
