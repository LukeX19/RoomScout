using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IBookingService _bookingService;

        public AvailabilityService(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<int> GetAvailabilityAsync(Hotel hotel, DateTime start, DateTime? end, RoomCode roomType)
        {
            var allRoomsOfType = hotel.Rooms
                .Where(r => r.RoomType == roomType)
                .Select(r => r.RoomId)
                .ToList();

            if (!allRoomsOfType.Any())
            {
                return 0;
            }

            var bookings = await _bookingService.GetAllBookingsAsync();
            var relevantBookings = bookings
                .Where(b =>
                    b.HotelId == hotel.Id &&
                    b.RoomType == roomType &&
                    DatesOverlap(start, end ?? start.AddDays(1), b.Arrival, b.Departure))
                .ToList();

            var bookedCount = relevantBookings.Count;

            var availableRooms = allRoomsOfType.Count - bookedCount;
            return availableRooms;
        }

        private bool DatesOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && start2 < end1;
        }
    }
}
