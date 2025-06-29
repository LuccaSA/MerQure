﻿using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using MerQure.Tools.Messages;
using Moq;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MerQure.Tools.Tests.Buses;

public class ConsumerTests : IDisposable
{
    private Mock<IConsumer> _mockMerQureConsumer;
    private Mock<IMessagingService> _mockMessagingService;

    private Consumer<TestMessage> _consumer;
    public ConsumerTests()
    {
        _mockMerQureConsumer = new Mock<IConsumer>();
        _mockMerQureConsumer.Setup(m => m.ConsumeAsync(It.IsAny<EventHandler<IMessagingEvent>>())).Callback((EventHandler<IMessagingEvent> action) =>
        {
            action(this, new Mock<IMessagingEvent>().Object);
        });

        _mockMessagingService = new Mock<IMessagingService>();
        _mockMessagingService.Setup(m => m.GetConsumerAsync(It.IsAny<string>())).ReturnsAsync(_mockMerQureConsumer.Object);

        _consumer = new Consumer<TestMessage>(_mockMessagingService.Object);

    }

    public void Dispose()
    {
        _mockMerQureConsumer = null;
        _mockMessagingService = null;
        _consumer = null;
    }

    [Fact]
    public void Consumer_OnMessageReceived_ShouldCallCallbackWithOriginalMessage()
    {
        //Arrange
        RetryMessage<TestMessage> messageInBody = GetFilledRetryMessage(0);

        Mock<IMessagingEvent> mockMessagingEvent = new Mock<IMessagingEvent>();
        mockMessagingEvent.Setup(p => p.Message.GetBody()).Returns(JsonConvert.SerializeObject(messageInBody));

        bool callbackCalled = false;
        TestMessage deliveredMessageFromCallback = null;
        //Act
        _consumer.OnMessageReceived((object sender, TestMessage deliveredMessage) =>
            {
                callbackCalled = true;
                deliveredMessageFromCallback = deliveredMessage;
            }
            , mockMessagingEvent.Object);

        //Assert
        Assert.True(callbackCalled);
        Assert.True(_consumer.RetryInformations.ContainsKey(deliveredMessageFromCallback.DeliveryTag));
    }

    [Fact]
    public async Task Consumer_AcknowlegdeDeliveredMessage_ShouldCallMerQureAcknowledgdeDelivredMessage()
    {
        //Arrange
        TestMessage testMessage = TestMessage.GetFilledTestMessage();

        //Act
        await _consumer.AcknowlegdeDeliveredMessageAsync(new Channel(""), testMessage);

        //Assert
        _mockMerQureConsumer.Verify(m => m.AcknowlegdeDeliveredMessageAsync(testMessage));
    }

    [Fact]
    public async Task Consumer_AcknowlegdeDeliveredMessage_ShouldRemoveMessageFromRetryInformationsList()
    {
        //Arrange
        RetryMessage<TestMessage> retryMessage = GetFilledRetryMessage(0);
        TestMessage testMessage = retryMessage.OriginalMessage;

        _consumer.RetryInformations.Add(testMessage.DeliveryTag, retryMessage.RetryInformations);

        //Act
        await _consumer.AcknowlegdeDeliveredMessageAsync(new Channel(""), testMessage);

        //Assert
        Assert.True(!_consumer.RetryInformations.ContainsKey(testMessage.DeliveryTag));
    }


    [Fact]
    public async Task Consumer_RejectDeliveredMessage_ShouldCallMerQureRejectDeliveredMessagee()
    {
        //Arrange
        TestMessage testMessage = TestMessage.GetFilledTestMessage();

        //Act
        await _consumer.RejectDeliveredMessageAsync(new Channel(""), testMessage);

        //Assert
        _mockMerQureConsumer.Verify(m => m.RejectDeliveredMessageAsync(testMessage));
    }

    [Fact]
    public async Task Consumer_RejectDeliveredMessage_ShouldRemoveMessageFromRetryInformationsList()
    {
        //Arrange
        RetryMessage<TestMessage> retryMessage = GetFilledRetryMessage(0);
        TestMessage testMessage = retryMessage.OriginalMessage;

        _consumer.RetryInformations.Add(testMessage.DeliveryTag, retryMessage.RetryInformations);

        //Act
        await _consumer.RejectDeliveredMessageAsync(new Channel(""), testMessage);

        //Assert
        Assert.True(!_consumer.RetryInformations.ContainsKey(testMessage.DeliveryTag));
    }

    private static RetryMessage<TestMessage> GetFilledRetryMessage(int numberOfRetry)
    {
        return new RetryMessage<TestMessage>
        {
            OriginalMessage = TestMessage.GetFilledTestMessage(),
            RetryInformations = new RetryInformations
            {
                NumberOfRetry = numberOfRetry
            }
        };
    }

}