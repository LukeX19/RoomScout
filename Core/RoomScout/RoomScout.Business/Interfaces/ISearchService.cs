using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Interfaces
{
    public interface ISearchService
    {
        Task<ICollection<(DateTime start, DateTime end, int available)>> SearchAvailabilityAsync(string hotelId, int numberOfDays, RoomCode roomType);
    }
}
