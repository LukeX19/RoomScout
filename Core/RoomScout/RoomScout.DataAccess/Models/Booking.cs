using Newtonsoft.Json;
using RoomScout.DataAccess.Converters;
using RoomScout.DataAccess.Enums;

namespace RoomScout.DataAccess.Models
{
    public class Booking
    {
        public string HotelId { get; set; }

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime Arrival { get; set; }

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime Departure { get; set; }

        public RoomCode RoomType { get; set; }
        public string RoomRate { get; set; }
    }
}
