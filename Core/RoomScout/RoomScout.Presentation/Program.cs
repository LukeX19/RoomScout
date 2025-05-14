using RoomScout.DataAccess;
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

            var hotelRepo = new HotelRepository(context);
            var bookingRepo = new BookingRepository(context);

            var hotels = await hotelRepo.GetAllAsync();
            Console.WriteLine("Hotels:");
            foreach (var hotel in hotels)
            {
                Console.WriteLine($"- {hotel.Id}: {hotel.Name} ({hotel.Rooms.Count} rooms)");
            }

            Console.WriteLine();

            var h1 = await hotelRepo.GetByIdAsync("H1");
            if (h1 != null)
            {
                Console.WriteLine($"Hotel {h1.Id} ({h1.Name}):");
                foreach (var room in h1.Rooms)
                {
                    Console.WriteLine($"- Room {room.RoomId} ({room.RoomType})");
                }
            }

            Console.WriteLine();

            var bookings = await bookingRepo.GetAllAsync();
            Console.WriteLine("Bookings:");
            foreach (var booking in bookings)
            {
                Console.WriteLine($"- {booking.HotelId}: {booking.RoomType}, {booking.Arrival:yyyy-MM-dd} -> {booking.Departure:yyyy-MM-dd}");
            }
        }
    }
}
