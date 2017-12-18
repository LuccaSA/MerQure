using MerQure.Tools.Buses;
using MerQure.Tools.Configurations;
using Moq;
using Xunit;

namespace MerQure.Tools.Tests.Buses
{
    public class ConsumerProviderTests
    {
        private Mock<IMessagingService> _mockMessagingService;
        private Mock<IConsumer> _mockMerQureConsumer;

        private ConsumerProvider _consumerProvider;

        public ConsumerProviderTests()
        {
            _mockMerQureConsumer = new Mock<IConsumer>();

            _mockMessagingService = new Mock<IMessagingService>();
            _mockMessagingService.Setup(m => m.GetConsumer(It.IsAny<string>())).Returns(_mockMerQureConsumer.Object);

            _consumerProvider = new ConsumerProvider(_mockMessagingService.Object);
        }

        [Fact]
        public void ConsumerProvider_Get_ShouldReturnNewConsumer_WhenChannelIsUnknown()
        {
            //Arrange
            string queueName = "testQueue"; 
            //Act
            _consumerProvider.Get(new Channel(queueName));
            //Assert
            _mockMessagingService.Verify(m => m.GetConsumer(queueName), Times.Once);
        }

        [Fact]
        public void ConsumerProvider_Get_ShouldReturnExistingConsumer_WhenChannelIsknown()
        {
            //Arrange
            string queueName = "testQueue";
            //create the existing consumer
            _consumerProvider.Get(new Channel(queueName));

            //Act
            //get same consumer
            _consumerProvider.Get(new Channel(queueName));

            //Assert
            // external get consumer should be call only once
            _mockMessagingService.Verify(m => m.GetConsumer(queueName), Times.Once);
        }
    }
}
