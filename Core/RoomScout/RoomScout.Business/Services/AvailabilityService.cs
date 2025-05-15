using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IHotelService _hotelService;
        private readonly IBookingService _bookingService;

        public AvailabilityService(IHotelService hotelService, IBookingService bookingService)
        {
            _hotelService = hotelService;
            _bookingService = bookingService;
        }

        public async Task<int> GetAvailabilityAsync(string hotelId, DateTime start, DateTime? end, RoomCode roomType)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                return 0;
            }

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
                    b.HotelId == hotelId &&
                    b.RoomType == roomType &&
                    DatesOverlap(start, end ?? start.AddDays(1), b.Arrival, b.Departure))
                .ToList();

            var maxRoomsBooked = relevantBookings.Count;

            var availableRooms = allRoomsOfType.Count - maxRoomsBooked;
            return Math.Max(availableRooms, 0);
        }

        private bool DatesOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && start2 < end1;
        }
    }
}
