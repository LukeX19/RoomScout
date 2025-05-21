using RoomScout.Presentation.Configuration;

namespace RoomScout.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Default paths for debugging in Visual Studio
            string hotelsPath = Path.Combine("SeedData", "hotels.json");
            string bookingsPath = Path.Combine("SeedData", "bookings.json");

            // CLI args: --hotels path --bookings path
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--hotels")
                {
                    hotelsPath = args[i + 1];
                }

                if (args[i] == "--bookings")
                {
                    bookingsPath = args[i + 1];
                }
            }

            IBootstrapper bootstrapper = new Bootstrapper();
            var dispatcher = await bootstrapper.BuildAsync(hotelsPath, bookingsPath);

            Console.WriteLine("Enter a command (Availability(...) or Search(...)), or press Enter to exit:");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                var result = await dispatcher.DispatchAsync(input);
                Console.WriteLine(result);
            }
        }
    }
}
