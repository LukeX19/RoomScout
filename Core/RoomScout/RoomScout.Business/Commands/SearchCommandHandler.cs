using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;
using System.Text.RegularExpressions;

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
            command = command.Trim();

            // Regex: optional spaces
            var pattern = @"^Search\s*\(";
            command = Regex.Replace(command, pattern, "", RegexOptions.IgnoreCase);

            // Remove the final closing parenthesis if it exists
            if (command.EndsWith(")"))
            {
                command = command.Substring(0, command.Length - 1).Trim();
            }

            var args = command.Split(',');

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
