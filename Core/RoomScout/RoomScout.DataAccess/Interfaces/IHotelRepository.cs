using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess.Interfaces
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task<Hotel?> GetByIdAsync(string hotelId);
    }
}
