using Moq;
using RoomScout.Business.Commands;
using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.UnitTests.Commands
{
    [TestClass]
    public class SearchCommandHandlerTests
    {
        private Mock<IHotelService> _hotelServiceMock;
        private Mock<ISearchService> _searchServiceMock;
        private SearchCommandHandler _handler;
        private Hotel _sampleHotel;

        [TestInitialize]
        public void Setup()
        {
            _hotelServiceMock = new Mock<IHotelService>();
            _searchServiceMock = new Mock<ISearchService>();
            _handler = new SearchCommandHandler(_hotelServiceMock.Object, _searchServiceMock.Object);

            _sampleHotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL }
                }
            };
        }

        [TestMethod]
        public async Task Returns_Formatted_Result_For_Valid_Command()
        {
            // Arrange
            _hotelServiceMock.Setup(h => h.GetHotelByIdAsync("H1")).ReturnsAsync(_sampleHotel);
            _searchServiceMock.Setup(s => s.SearchAvailabilityAsync(_sampleHotel, 3, RoomCode.SGL))
                .ReturnsAsync(new List<(DateTime start, DateTime end, int available)>
                {
                    (new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), 2),
                    (new DateTime(2025, 9, 5), new DateTime(2025, 9, 7), 1)
                });

            // Act
            var result = await _handler.ExecuteAsync("Search(H1, 3, SGL)");

            // Assert
            Assert.AreEqual("(20250901-20250902, 2), (20250905-20250907, 1)", result);
        }

        [TestMethod]
        public async Task Handles_Whitespace_And_Casing()
        {
            // Arrange
            _hotelServiceMock.Setup(h => h.GetHotelByIdAsync("H1")).ReturnsAsync(_sampleHotel);
            _searchServiceMock.Setup(s => s.SearchAvailabilityAsync(_sampleHotel, 1, RoomCode.SGL))
                .ReturnsAsync(new List<(DateTime start, DateTime end, int available)>
                {
                    (DateTime.Today, DateTime.Today.AddDays(1), 1)
                });

            // Act
            var result = await _handler.ExecuteAsync(" search ( H1 , 1 , sgl ) ");

            // Assert
            var expected = $"({DateTime.Today:yyyyMMdd}-{DateTime.Today.AddDays(1):yyyyMMdd}, 1)";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task Returns_Empty_When_No_Availability()
        {
            // Arrange
            _hotelServiceMock.Setup(h => h.GetHotelByIdAsync("H1")).ReturnsAsync(_sampleHotel);
            _searchServiceMock.Setup(s => s.SearchAvailabilityAsync(_sampleHotel, 3, RoomCode.SGL))
                .ReturnsAsync(new List<(DateTime start, DateTime end, int available)>());

            // Act
            var result = await _handler.ExecuteAsync("Search(H1, 3, SGL)");

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_RoomType()
        {
            // Act
            var result = await _handler.ExecuteAsync("Search(H1, 30, XXXX)");

            // Assert
            Assert.AreEqual("Invalid room type: 'XXXX'", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_NumberOfDays()
        {
            // Act
            var result = await _handler.ExecuteAsync("Search(H1, ten, SGL)");

            // Assert
            Assert.AreEqual("Invalid number of days: 'ten'", result);
        }

        [TestMethod]
        public async Task Returns_Error_For_Invalid_Command_Format()
        {
            // Act
            var result = await _handler.ExecuteAsync("Search(H1, 30)");

            // Assert
            Assert.AreEqual("Invalid command format. Expected: Search(H1, 30, SGL)", result);
        }

        [TestMethod]
        public async Task Returns_Error_If_Hotel_Not_Found()
        {
            // Arrange
            _hotelServiceMock.Setup(h => h.GetHotelByIdAsync("H99")).ReturnsAsync((Hotel?)null);

            // Act
            var result = await _handler.ExecuteAsync("Search(H99, 30, SGL)");

            // Assert
            Assert.AreEqual("Hotel 'H99' was not found.", result);
        }

        [TestMethod]
        public async Task Returns_Error_If_RoomType_Not_Found_In_Hotel()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "201", RoomType = RoomCode.DBL }
                }
            };

            // Act
            _hotelServiceMock.Setup(h => h.GetHotelByIdAsync("H1")).ReturnsAsync(hotel);

            // Assert
            var result = await _handler.ExecuteAsync("Search(H1, 5, SGL)");
            Assert.AreEqual("Room type 'SGL' was not found in hotel 'H1'.", result);
        }
    }
}
