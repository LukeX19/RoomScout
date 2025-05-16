using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Interfaces
{
    public interface IAvailabilityService
    {
        Task<int> GetAvailabilityAsync(string hotelId, DateTime start, DateTime? end, RoomCode roomType);
    }
}
