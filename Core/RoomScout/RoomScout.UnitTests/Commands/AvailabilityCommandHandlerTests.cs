using Moq;
using RoomScout.Business.Commands;
using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.UnitTests.Commands
{
    [TestClass]
    public class AvailabilityCommandHandlerTests
    {
        private Mock<IHotelService> _mockHotelService;
        private Mock<IAvailabilityService> _mockAvailabilityService;
        private AvailabilityCommandHandler _handler;
        private Hotel _sampleHotel;

        [TestInitialize]
        public void Setup()
        {
            _mockHotelService = new Mock<IHotelService>();
            _mockAvailabilityService = new Mock<IAvailabilityService>();

            _handler = new AvailabilityCommandHandler(_mockHotelService.Object, _mockAvailabilityService.Object);

            _sampleHotel = new Hotel
            {
                Id = "H1",
                Name = "Hotel California",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL },
                    new Room { RoomId = "201", RoomType = RoomCode.DBL },
                }
            };
        }

        [TestMethod]
        public async Task Returns_Availability_For_Valid_SingleDate_Command()
        {
            // Arrange
            _mockHotelService.Setup(h => h.GetHotelByIdAsync("H1"))
                .ReturnsAsync(_sampleHotel);

            _mockAvailabilityService.Setup(a =>
                a.GetAvailabilityAsync(_sampleHotel, new DateTime(2025, 9, 1), null, RoomCode.SGL))
                .ReturnsAsync(2);

            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 20250901, SGL)");

            // Assert
            Assert.AreEqual("2", result);
        }

        [TestMethod]
        public async Task Returns_Availability_For_Valid_DateRange_Command()
        {
            // Arrange
            _mockHotelService.Setup(h => h.GetHotelByIdAsync("H1"))
                .ReturnsAsync(_sampleHotel);

            _mockAvailabilityService.Setup(a =>
                a.GetAvailabilityAsync(_sampleHotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 3), RoomCode.DBL))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 20250901-20250903, DBL)");

            // Assert
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_RoomType()
        {
            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 20250901, XXXX)");

            // Assert
            Assert.AreEqual("Invalid room type: 'XXXX'", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_Date_Format()
        {
            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 01-09-2025, SGL)");

            // Assert
            Assert.AreEqual("Invalid date range format: '01-09-2025'", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_Date()
        {
            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 20250932, SGL)");

            // Assert
            Assert.AreEqual("Invalid start date: '20250932'", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_Command_Format()
        {
            // Act
            var result = await _handler.ExecuteAsync("Availability(H1, 20250901)");

            // Assert
            Assert.AreEqual("Invalid command format. Expected: Availability(H1, 20250901[-20250903], DBL)", result);
        }

        [TestMethod]
        public async Task Returns_Error_If_Hotel_Not_Found()
        {
            // Arrange
            _mockHotelService.Setup(h => h.GetHotelByIdAsync("H99"))
                .ReturnsAsync((Hotel?)null);

            // Act
            var result = await _handler.ExecuteAsync("Availability(H99, 20250901, SGL)");

            // Assert
            Assert.AreEqual("Hotel 'H99' was not found.", result);
        }

        [TestMethod]
        public async Task Returns_Error_If_RoomType_Not_Found()
        {
            // Arrange
            var hotelWithoutSGL = new Hotel
            {
                Id = "H2",
                Name = "Another Hotel",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "301", RoomType = RoomCode.DBL }
                }
            };

            _mockHotelService.Setup(h => h.GetHotelByIdAsync("H2"))
                .ReturnsAsync(hotelWithoutSGL);

            // Act
            var result = await _handler.ExecuteAsync("Availability(H2, 20250901, SGL)");

            // Assert
            Assert.AreEqual("Room type 'SGL' was not found in hotel 'H2'.", result);
        }

        [TestMethod]
        public async Task Handles_Whitespace_And_Case_Insensitivity()
        {
            // Arrange
            _mockHotelService.Setup(h => h.GetHotelByIdAsync("H1"))
                .ReturnsAsync(_sampleHotel);

            _mockAvailabilityService.Setup(a =>
                a.GetAvailabilityAsync(_sampleHotel, new DateTime(2025, 9, 1), null, RoomCode.SGL))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.ExecuteAsync("availability ( H1 , 20250901 , sgl )");

            // Assert
            Assert.AreEqual("1", result);
        }
    }
}
