using Moq;
using RoomScout.Business.Interfaces;
using RoomScout.Presentation.Dispatcher;

namespace RoomScout.UnitTests.Dispatcher
{
    [TestClass]
    public class CommandDispatcherTests
    {
        private CommandDispatcher _dispatcher;
        private Mock<ICommandHandler> _mockAvailabilityHandler;
        private Mock<ICommandHandler> _mockSearchHandler;

        [TestInitialize]
        public void Setup()
        {
            _mockAvailabilityHandler = new Mock<ICommandHandler>();
            _mockSearchHandler = new Mock<ICommandHandler>();

            _dispatcher = new CommandDispatcher();
            _dispatcher.Register("Availability", _mockAvailabilityHandler.Object);
            _dispatcher.Register("Search", _mockSearchHandler.Object);
        }

        [TestMethod]
        public async Task DispatchAsync_CallsCorrectHandler_ForAvailability()
        {
            // Arrange
            _mockAvailabilityHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync("Handled Availability");

            // Act
            var result = await _dispatcher.DispatchAsync("Availability(H1, 20250901, SGL)");

            // Assert
            Assert.AreEqual("Handled Availability", result);
        }

        [TestMethod]
        public async Task DispatchAsync_CallsCorrectHandler_ForSearch()
        {
            // Arrange
            _mockSearchHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync("Handled Search");

            // Act
            var result = await _dispatcher.DispatchAsync("Search(H1, 30, DBL)");

            // Assert
            Assert.AreEqual("Handled Search", result);
        }

        [TestMethod]
        public async Task DispatchAsync_IsCaseInsensitive()
        {
            // Arrange
            _mockAvailabilityHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync("OK");

            // Act
            var result = await _dispatcher.DispatchAsync("availability(H1, 20250901, SGL)");

            // Assert
            Assert.AreEqual("OK", result);
        }

        [TestMethod]
        public async Task DispatchAsync_Allows_Whitespace_Between_Command_And_Parenthesis()
        {
            // Arrange
            _mockSearchHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync("Good");

            // Act
            var result = await _dispatcher.DispatchAsync("Search ( H1 , 30 , DBL )");

            // Assert
            Assert.AreEqual("Good", result);
        }

        [TestMethod]
        public async Task DispatchAsync_ReturnsError_ForUnsupportedCommand()
        {
            // Act
            var result = await _dispatcher.DispatchAsync("UnknownCommand(H1, 30, SGL)");

            // Assert
            Assert.AreEqual("Unsupported command. Only Availability(...) and Search(...) are supported right now.", result);
        }

        [TestMethod]
        public async Task DispatchAsync_ReturnsError_ForEmptyInput()
        {
            // Act
            var result = await _dispatcher.DispatchAsync("   ");

            // Assert
            Assert.AreEqual("Empty command.", result);
        }

        [TestMethod]
        public async Task DispatchAsync_HandlesWhitespaceAroundInput()
        {
            // Arrange
            _mockAvailabilityHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync("Clean");

            // Act
            var result = await _dispatcher.DispatchAsync("   Availability( H1 , 20250901 , SGL )   ");

            // Assert
            Assert.AreEqual("Clean", result);
        }
    }
}
