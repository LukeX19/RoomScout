using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess.Interfaces
{
    public interface IHotelRepository
    {
        Task<ICollection<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(string hotelId);
    }
}
