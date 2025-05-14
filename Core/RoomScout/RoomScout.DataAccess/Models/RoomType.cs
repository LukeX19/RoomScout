using RoomScout.DataAccess.Enums;

namespace RoomScout.DataAccess.Models
{
    public class RoomType
    {
        public RoomCode Code { get; set; }
        public string Description { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> Features { get; set; }
    }
}
