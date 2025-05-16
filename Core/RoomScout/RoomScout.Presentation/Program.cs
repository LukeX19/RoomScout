using RoomScout.Business.Commands;
using RoomScout.Business.Interfaces;
using RoomScout.Business.Services;
using RoomScout.DataAccess;
using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Repositories;

namespace RoomScout.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hotelsPath = Path.Combine("SeedData", "hotels.json");
            var bookingsPath = Path.Combine("SeedData", "bookings.json");

            var context = new DataContext();
            await context.InitializeAsync(hotelsPath, bookingsPath);

            // Set up repositories
            IHotelRepository hotelRepo = new HotelRepository(context);
            IBookingRepository bookingRepo = new BookingRepository(context);

            // Set up services
            IHotelService hotelService = new HotelService(hotelRepo);
            IBookingService bookingService = new BookingService(bookingRepo);
            IAvailabilityService availabilityService = new AvailabilityService(hotelService, bookingService);
            ISearchService searchService = new SearchService(availabilityService);

            // Set up command handlers
            var availabilityHandler = new AvailabilityCommandHandler(availabilityService);
            var searchHandler = new SearchCommandHandler(searchService);

            Console.WriteLine("Enter a command (Availability(...) or Search(...)), or press Enter to exit:");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                string result;
                if (input.StartsWith("Availability(", StringComparison.OrdinalIgnoreCase))
                {
                    result = await availabilityHandler.ExecuteAsync(input);
                }
                else if (input.StartsWith("Search(", StringComparison.OrdinalIgnoreCase))
                {
                    result = await searchHandler.ExecuteAsync(input);
                }
                else
                {
                    result = "Unsupported command. Only Availability(...) and Search(...) are supported right now.";
                }

                Console.WriteLine(result);
            }
        }
    }
}
