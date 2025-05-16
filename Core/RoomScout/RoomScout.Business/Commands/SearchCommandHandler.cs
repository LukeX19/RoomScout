using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Commands
{
    public class SearchCommandHandler : ICommandHandler
    {
        private readonly ISearchService _searchService;

        public SearchCommandHandler(ISearchService searchService)
        {
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

            var results = await _searchService.SearchAvailabilityAsync(hotelId, numberOfDays, roomType);
            if (!results.Any())
            {
                return "";
            }

            var formatted = results.Select(r => $"({r.start:yyyyMMdd}-{r.end:yyyyMMdd}, {r.available})");

            return string.Join(", ", formatted);
        }
    }
}
