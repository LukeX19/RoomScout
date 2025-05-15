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

            // Set up command handlers
            var availabilityHandler = new AvailabilityCommandHandler(availabilityService);

            Console.WriteLine("Enter an Availability command or press Enter to exit:");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                if (input.StartsWith("Availability(", StringComparison.OrdinalIgnoreCase))
                {
                    var result = await availabilityHandler.ExecuteAsync(input);
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Unsupported command. Only Availability(...) is supported right now.");
                }
            }
        }
    }
}
