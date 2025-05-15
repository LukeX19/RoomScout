using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Models;

namespace RoomScout.DataAccess.Repositories
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(DataContext context) : base(context.Hotels) { }

        public async Task<Hotel?> GetByIdAsync(string hotelId)
        {
            var hotel = _entities.FirstOrDefault(h => h.Id == hotelId);

            return await Task.FromResult(hotel);
        }
    }
}
