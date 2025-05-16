using RoomScout.Business.Interfaces;
using RoomScout.DataAccess.Enums;

namespace RoomScout.Business.Services
{
    public class SearchService : ISearchService
    {
        private readonly IAvailabilityService _availabilityService;

        public SearchService(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        public async Task<ICollection<(DateTime start, DateTime end, int available)>> SearchAvailabilityAsync(string hotelId, int numberOfDays, RoomCode roomType)
        {
            //var today = new DateTime(2025, 9, 1);
            var today = DateTime.Today;
            var results = new List<(DateTime start, DateTime end, int available)>();

            DateTime? currentStart = null;
            DateTime? currentEnd = null;
            int? currentAvailability = null;

            for (int i = 0; i < numberOfDays; i++)
            {
                var start = today.AddDays(i);
                var end = start.AddDays(1);

                var available = await _availabilityService.GetAvailabilityAsync(hotelId, start, end, roomType);
                if (available <= 0)
                {
                    // Close existing range
                    if (currentStart != null)
                    {
                        results.Add((currentStart.Value, currentEnd.Value, currentAvailability.Value));
                        currentStart = null;
                        currentEnd = null;
                        currentAvailability = null;
                    }

                    continue;
                }

                // Start new range
                if (currentStart == null)
                {
                    currentStart = start;
                    currentEnd = end;
                    currentAvailability = available;
                }
                // Continue existing range and extend it if availability matches
                else if (available == currentAvailability && start == currentEnd)
                {
                    currentEnd = end;
                }
                // Availability changed, so end old range
                else
                {
                    results.Add((currentStart.Value, currentEnd.Value, currentAvailability.Value));
                    currentStart = start;
                    currentEnd = end;
                    currentAvailability = available;
                }
            }

            // If last range is open, finish it
            if (currentStart != null)
            {
                results.Add((currentStart.Value, currentEnd.Value, currentAvailability.Value));
            }

            return results;
        }
    }
}
