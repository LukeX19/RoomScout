using RoomScout.DataAccess.Enums;
using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Interfaces
{
    public interface ISearchService
    {
        Task<ICollection<(DateTime start, DateTime end, int available)>> SearchAvailabilityAsync(Hotel hotel, int numberOfDays, RoomCode roomType);
    }
}
