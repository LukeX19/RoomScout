using RoomScout.Business.Commands;
using RoomScout.Business.Interfaces;
using RoomScout.Business.Services;
using RoomScout.DataAccess.Interfaces;
using RoomScout.DataAccess.Repositories;
using RoomScout.DataAccess;
using RoomScout.Presentation.Dispatcher;

namespace RoomScout.Presentation.Configuration
{
    public class Bootstrapper : IBootstrapper
    {
        public async Task<ICommandDispatcher> BuildAsync()
        {
            var hotelsPath = Path.Combine("SeedData", "hotels.json");
            var bookingsPath = Path.Combine("SeedData", "bookings.json");

            var context = new DataContext();
            await context.InitializeAsync(hotelsPath, bookingsPath);

            // Repositories
            IHotelRepository hotelRepo = new HotelRepository(context);
            IBookingRepository bookingRepo = new BookingRepository(context);

            // Services
            IHotelService hotelService = new HotelService(hotelRepo);
            IBookingService bookingService = new BookingService(bookingRepo);
            IAvailabilityService availabilityService = new AvailabilityService(hotelService, bookingService);
            ISearchService searchService = new SearchService(availabilityService);

            // Handlers
            var availabilityHandler = new AvailabilityCommandHandler(availabilityService);
            var searchHandler = new SearchCommandHandler(searchService);

            // Dispatcher
            var dispatcher = new CommandDispatcher();
            dispatcher.Register("Availability", availabilityHandler);
            dispatcher.Register("Search", searchHandler);

            return dispatcher;
        }
    }
}
