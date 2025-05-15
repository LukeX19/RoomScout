using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public Task<ICollection<Booking>> GetAllBookingsAsync()
        {
            return _bookingRepository.GetAllAsync();
        }
    }
}
