using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Interfaces
{
    public interface IBookingService
    {
        Task<ICollection<Booking>> GetAllBookingsAsync();
    }
}
