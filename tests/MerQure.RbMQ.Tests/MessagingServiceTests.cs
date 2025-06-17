using System;
using System.Collections.Generic;
using Xunit;

namespace MerQure.RbMQ.Tests;

public class MessagingServiceTests
{
    [Fact]
    public void DeclareExchangeFailWithEmptyName()
    {
        var messagingService = new MessagingService(null,null);

        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareExchangeAsync(null));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareExchangeAsync("    "));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public void DeclareQueueFailWithEmptyName(bool isQuorum)
    {
        var messagingService = new MessagingService(null, null);

        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareQueueAsync(null, isQuorum: isQuorum));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareQueueAsync("    ", isQuorum: isQuorum));
    }
    public static IEnumerable<object[]> IsQuorumPossibleValues =>
        new List<object[]>
        {
            new object[] { true },
            new object[] { false }
        };

    [Fact]
    public void DeclareBindingAsyncFailWithEmptyParameters()
    {
        var messagingService = new MessagingService(null, null);

        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync(null, "queue", "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", null, "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("", "queue", "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", "", "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", "queue", null));
    }

    [Fact]
    public void CancelBindingFailWithEmptyParameters()
    {
        var messagingService = new MessagingService(null, null);

        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync(null, "queue", "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync("exhange", null, "routing"));
        Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync("exhange", "queue", null));
    }
}