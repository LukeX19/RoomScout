using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;
using System.Globalization;

namespace RoomScout.Business.Commands
{
    public class AvailabilityCommandHandler : ICommandHandler
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityCommandHandler(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        public async Task<string> ExecuteAsync(string command)
        {
            var cleaned = command
                .Replace("Availability(", "", StringComparison.OrdinalIgnoreCase)
                .Replace(")", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            var args = cleaned.Split(',');

            if (args.Length != 3)
            {
                return "Invalid command format. Expected: Availability(H1, 20250901[-20250903], DBL)";
            }

            var hotelId = args[0].Trim();
            var dateInput = args[1].Trim();
            var roomTypeStr = args[2].Trim().ToUpper();

            if (!Enum.TryParse<RoomCode>(roomTypeStr, true, out var roomType))
                return $"Invalid room type: '{roomTypeStr}'";

            var dates = dateInput.Split('-');
            if (!DateTime.TryParseExact(dates[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
                return $"Invalid start date: '{dates[0]}'";

            DateTime? end = null;
            if (dates.Length == 2)
            {
                if (!DateTime.TryParseExact(dates[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEnd))
                    return $"Invalid end date: '{dates[1]}'";

                end = parsedEnd;
            }

            var result = await _availabilityService.GetAvailabilityAsync(hotelId, start, end, roomType);

            return result.ToString();
        }
    }
}
