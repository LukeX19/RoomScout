using RoomScout.Presentation.Configuration;

namespace RoomScout.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IBootstrapper bootstrapper = new Bootstrapper();
            var dispatcher = await bootstrapper.BuildAsync();

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
