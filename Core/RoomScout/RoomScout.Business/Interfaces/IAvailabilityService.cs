using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Interfaces
{
    public interface IAvailabilityService
    {
        Task<int> GetAvailabilityAsync(Hotel hotel, DateTime start, DateTime? end, RoomCode roomType);
    }
}
