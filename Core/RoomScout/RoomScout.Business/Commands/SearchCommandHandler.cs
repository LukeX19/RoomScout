using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Commands
{
    public class SearchCommandHandler : ICommandHandler
    {
        private readonly IHotelService _hotelService;
        private readonly ISearchService _searchService;

        public SearchCommandHandler(IHotelService hotelService, ISearchService searchService)
        {
            _hotelService = hotelService;
            _searchService = searchService;
        }

        public async Task<string> ExecuteAsync(string command)
        {
            var cleaned = command
                .Replace("Search(", "", StringComparison.OrdinalIgnoreCase)
                .Replace(")", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            var args = cleaned.Split(',');

            if (args.Length != 3)
            {
                return "Invalid command format. Expected: Search(H1, 30, SGL)";
            }

            var hotelId = args[0].Trim();
            var numberOfDaysStr = args[1].Trim();
            var roomTypeStr = args[2].Trim().ToUpper();

            if (!int.TryParse(numberOfDaysStr, out var numberOfDays))
            {
                return $"Invalid number of days: '{numberOfDaysStr}'";
            }

            if (!Enum.TryParse<RoomCode>(roomTypeStr, true, out var roomType))
            {
                return $"Invalid room type: '{roomTypeStr}'";
            }

            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                return $"Hotel '{hotelId}' was not found.";
            }

            var hasRoomType = hotel.Rooms.Any(r => r.RoomType == roomType);
            if (!hasRoomType)
            {
                return $"Room type '{roomType}' was not found in hotel '{hotelId}'.";
            }

            var results = await _searchService.SearchAvailabilityAsync(hotel, numberOfDays, roomType);
            if (!results.Any())
            {
                return "";
            }

            var formatted = results.Select(r => $"({r.start:yyyyMMdd}-{r.end:yyyyMMdd}, {r.available})");

            return string.Join(", ", formatted);
        }
    }
}
