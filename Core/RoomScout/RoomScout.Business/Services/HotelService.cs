using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Models;

namespace RoomScout.Business.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public Task<ICollection<Hotel>> GetAllHotelsAsync()
        {
            return _hotelRepository.GetAllAsync();
        }

        public Task<Hotel?> GetHotelByIdAsync(string id)
        {
            return _hotelRepository.GetByIdAsync(id);
        }
    }
}
