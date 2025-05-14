using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ICollection<Booking> bookings) : base(bookings) { }
    }
}
