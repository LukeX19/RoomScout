using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Interfaces
{
    public interface IHotelService
    {
        Task<ICollection<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(string id);
    }
}
