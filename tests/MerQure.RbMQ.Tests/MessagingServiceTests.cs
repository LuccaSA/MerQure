using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MerQure.RbMQ.Tests;

public class MessagingServiceTests
{
    [Fact]
    public async Task DeclareExchangeFailWithEmptyName()
    {
        var messagingService = new MessagingService(null,null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareExchangeAsync(null));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareExchangeAsync("    "));
    }

    [Theory]
    [MemberData(nameof(IsQuorumPossibleValues))]
    public async Task DeclareQueueFailWithEmptyName(bool isQuorum)
    {
        var messagingService = new MessagingService(null, null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareQueueAsync(null, isQuorum: isQuorum));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareQueueAsync("    ", isQuorum: isQuorum));
    }
    public static IEnumerable<object[]> IsQuorumPossibleValues =>
        new List<object[]>
        {
            new object[] { true },
            new object[] { false }
        };

    [Fact]
    public async Task DeclareBindingAsyncFailWithEmptyParameters()
    {
        var messagingService = new MessagingService(null, null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync(null, "queue", "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", null, "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("", "queue", "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", "", "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.DeclareBindingAsync("exhange", "queue", null));
    }

    [Fact]
    public async Task CancelBindingFailWithEmptyParameters()
    {
        var messagingService = new MessagingService(null, null);

        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync(null, "queue", "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync("exhange", null, "routing"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => messagingService.CancelBindingAsync("exhange", "queue", null));
    }
}