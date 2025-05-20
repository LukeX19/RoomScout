using Moq;
using RoomScout.Business.Interfaces;
using RoomScout.Business.Services;
using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.UnitTests.Services
{
    [TestClass]
    public class SearchServiceTests
    {
        private Mock<IAvailabilityService> _availabilityServiceMock;
        private SearchService _searchService;
        private Hotel _sampleHotel;

        [TestInitialize]
        public void Setup()
        {
            _availabilityServiceMock = new Mock<IAvailabilityService>();
            _searchService = new SearchService(_availabilityServiceMock.Object);

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
        public async Task SearchAvailabilityAsync_ReturnsCorrectGrouping()
        {
            // Arrange
            _availabilityServiceMock
                .Setup(a => a.GetAvailabilityAsync(_sampleHotel, It.IsAny<DateTime>(), It.IsAny<DateTime>(), RoomCode.SGL))
                .ReturnsAsync((Hotel h, DateTime start, DateTime end, RoomCode type) =>
                {
                    var day = (start - DateTime.Today).Days;
                    return (day < 3) ? 2 : 0;
                });

            // Act
            var result = await _searchService.SearchAvailabilityAsync(_sampleHotel, 5, RoomCode.SGL);

            // Assert
            Assert.AreEqual(1, result.Count);
            var (start, end, availability) = result.First();
            Assert.AreEqual(DateTime.Today, start);
            Assert.AreEqual(DateTime.Today.AddDays(3), end);
            Assert.AreEqual(2, availability);
        }

        [TestMethod]
        public async Task SearchAvailabilityAsync_ReturnsEmpty_WhenNoAvailability()
        {
            // Arrange
            _availabilityServiceMock
                .Setup(a => a.GetAvailabilityAsync(_sampleHotel, It.IsAny<DateTime>(), It.IsAny<DateTime>(), RoomCode.SGL))
                .ReturnsAsync(0);

            // Act
            var result = await _searchService.SearchAvailabilityAsync(_sampleHotel, 5, RoomCode.SGL);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SearchAvailabilityAsync_SplitsRanges_WhenAvailabilityDiffers()
        {
            // Arrange
            _availabilityServiceMock
                .Setup(a => a.GetAvailabilityAsync(_sampleHotel, It.IsAny<DateTime>(), It.IsAny<DateTime>(), RoomCode.SGL))
                .ReturnsAsync((Hotel h, DateTime start, DateTime end, RoomCode code) =>
                {
                    var offset = (start - DateTime.Today).Days;
                    return offset + 1;
                });

            // Act
            var result = (await _searchService.SearchAvailabilityAsync(_sampleHotel, 3, RoomCode.SGL)).ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].available);
            Assert.AreEqual(2, result[1].available);
            Assert.AreEqual(3, result[2].available);
        }

        [TestMethod]
        public async Task SearchAvailabilityAsync_ClosesFinalOpenRange()
        {
            // Arrange
            _availabilityServiceMock
                .Setup(a => a.GetAvailabilityAsync(_sampleHotel, It.IsAny<DateTime>(), It.IsAny<DateTime>(), RoomCode.SGL))
                .ReturnsAsync(1);

            // Act
            var result = (await _searchService.SearchAvailabilityAsync(_sampleHotel, 2, RoomCode.SGL)).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(DateTime.Today, result[0].start);
            Assert.AreEqual(DateTime.Today.AddDays(2), result[0].end);
        }

        [TestMethod]
        public async Task SearchAvailabilityAsync_ReturnsEmpty_WhenZeroDaysPassed()
        {
            // Act
            var result = await _searchService.SearchAvailabilityAsync(_sampleHotel, 0, RoomCode.SGL);

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
