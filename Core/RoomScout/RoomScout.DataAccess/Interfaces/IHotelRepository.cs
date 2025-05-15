using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess.Interfaces
{
    public interface IHotelRepository
    {
        Task<Hotel?> GetByIdAsync(string hotelId);
    }
}
