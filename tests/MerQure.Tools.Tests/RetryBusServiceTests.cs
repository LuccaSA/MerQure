using MerQure.Tools.Configurations;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MerQure.Tools.Tests;

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
        _mockMessagingService
            .Setup(service => service.DeclareQueueAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((string queueName, bool isQuorum) => isQuorum ? queueName + "-q" : queueName);

        _retryBusService = new RetryBusService(_mockMessagingService.Object);
    }

    public void Dispose()
    {
        _mockMessagingService = null;
        _retryBusService = null;
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public void RetryBusService_CreateNewBus_ShouldThrow_WhenEmptyChannels(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();
        retryStrategy.Channels = new List<Channel>();

        //Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public void RetryBusService_CreateNewBus_ShouldThrow_WhenNullChannels(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();
        retryStrategy.Channels = null;

        //Act && Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareTopicExchangeWithBusName(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareExchangeAsync(retryStrategy.BusName, Constants.ExchangeTypeTopic));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareQueueForeachChannel(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareQueueAsync(_channelNames[0], isQuorum));
        _mockMessagingService.Verify(m => m.DeclareQueueAsync(_channelNames[1], isQuorum));
    }

    [Fact]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareBindingForeachChannel()
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: false);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareBindingAsync(retryStrategy.BusName, $"{_channelNames[0]}", It.IsAny<string>()));
        _mockMessagingService.Verify(m => m.DeclareBindingAsync(retryStrategy.BusName, $"{_channelNames[1]}", It.IsAny<string>()));
    }

    [Fact]
    public async Task RetryBusService_CreateNewBus_quorum_ShouldDeclareBindingForeachChannel()
    {
        //Arrange
        var retryStrategy = GetMinimalValidConfiguration();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: true);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareBindingAsync(retryStrategy.BusName, $"{_channelNames[0]}-q", It.IsAny<string>()));
        _mockMessagingService.Verify(m => m.DeclareBindingAsync(retryStrategy.BusName, $"{_channelNames[1]}-q", It.IsAny<string>()));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareDirectRetryExchangeWithBusName_WhenRetryStrategyContainsDelays(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetValidConfigurationWithDelay();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareExchangeAsync(It.Is<string>(exchangeName => exchangeName.Contains(retryStrategy.BusName) && exchangeName.Contains(RetryStrategyConfiguration.RetryExchangeSuffix)),
            Constants.ExchangeTypeDirect));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareDealLetterQueueForeachChannelAndDelay_WhenRetryStrategyContainsDelays(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetValidConfigurationWithDelay();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum);

        //Assert
        foreach(int delay in retryStrategy.DelaysInMsBetweenEachRetry)
        {
            _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicyAsync(It.Is<string>(queueName => queueName.Contains(_channelNames[0])),
                retryStrategy.BusName,
                delay,
                null,
                isQuorum));
            _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicyAsync(It.Is<string>(queueName => queueName.Contains(_channelNames[1])),
                retryStrategy.BusName,
                delay,
                null,
                isQuorum));
        }
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task RetryBusService_CreateNewBus_ShouldDeclareDealLetterQueueForeachChannelAndDelay_WhenRetryStrategyContainsPublishingDelay(bool isQuorum)
    {
        //Arrange
        var retryStrategy = GetValidConfigurationWithPublishingDelay();

        //Act
        await _retryBusService.CreateNewBusAsync<TestMessage>(retryStrategy, isQuorum: isQuorum);

        //Assert
        _mockMessagingService.Verify(m => m.DeclareQueueWithDeadLetterPolicyAsync(It.Is<string>(queueName => queueName.Contains(_channelNames[0])),
            retryStrategy.BusName,
            retryStrategy.DeliveryDelayInMilliseconds,
            null,
            isQuorum));
    }

    public static IEnumerable<object[]> IsQuorumPossibleValues =>
        new List<object[]>
        {
            new object[] { true },
            new object[] { false }
        };

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