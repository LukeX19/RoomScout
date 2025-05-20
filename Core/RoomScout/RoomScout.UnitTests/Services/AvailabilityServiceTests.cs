using Moq;
using RoomScout.Business.Interfaces;
using RoomScout.Business.Services;
using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.UnitTests.Services
{
    [TestClass]
    public class AvailabilityServiceTests
    {
        private Mock<IBookingService> _bookingServiceMock;
        private AvailabilityService _service;

        [TestInitialize]
        public void Setup()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _service = new AvailabilityService(_bookingServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_ReturnsCorrectCount_WhenNoBookings()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL }
                }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), RoomCode.SGL);

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_ReturnsZero_WhenAllRoomsBooked()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL }
                }
            };

            var bookings = new List<Booking>
            {
                new Booking { HotelId = "H1", RoomType = RoomCode.SGL, Arrival = new DateTime(2025, 9, 1), Departure = new DateTime(2025, 9, 2) },
                new Booking { HotelId = "H1", RoomType = RoomCode.SGL, Arrival = new DateTime(2025, 9, 1), Departure = new DateTime(2025, 9, 2) }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), RoomCode.SGL);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_ReturnsCorrectCount_WhenSomeRoomsBooked()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL },
                    new Room { RoomId = "103", RoomType = RoomCode.SGL }
                }
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    HotelId = "H1",
                    RoomType = RoomCode.SGL,
                    Arrival = new DateTime(2025, 9, 1),
                    Departure = new DateTime(2025, 9, 2)
                }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), RoomCode.SGL);

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_IgnoresBookings_OutsideDateRange()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL }
                }
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    HotelId = "H1",
                    RoomType = RoomCode.SGL,
                    Arrival = new DateTime(2025, 9, 10),
                    Departure = new DateTime(2025, 9, 12)
                }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), RoomCode.SGL);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_Handles_NullEndDate_AsSingleDay()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL },
                    new Room { RoomId = "102", RoomType = RoomCode.SGL }
                }
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    HotelId = "H1",
                    RoomType = RoomCode.SGL,
                    Arrival = new DateTime(2025, 9, 1),
                    Departure = new DateTime(2025, 9, 2)
                }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), null, RoomCode.SGL);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task GetAvailabilityAsync_IgnoresBookings_FromOtherHotels()
        {
            // Arrange
            var hotel = new Hotel
            {
                Id = "H1",
                Rooms = new List<Room>
                {
                    new Room { RoomId = "101", RoomType = RoomCode.SGL }
                }
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    HotelId = "H2",
                    RoomType = RoomCode.SGL,
                    Arrival = new DateTime(2025, 9, 1),
                    Departure = new DateTime(2025, 9, 2)
                }
            };

            _bookingServiceMock
                .Setup(b => b.GetAllBookingsAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAvailabilityAsync(hotel, new DateTime(2025, 9, 1), new DateTime(2025, 9, 2), RoomCode.SGL);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
