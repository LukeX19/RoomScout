using RoomScout.DataAccess.Enums;

namespace RoomScout.DataAccess.Models
{
    public class Booking
    {
        public string HotelId { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public RoomCode RoomType { get; set; }
        public string RoomRate { get; set; }
    }
}
