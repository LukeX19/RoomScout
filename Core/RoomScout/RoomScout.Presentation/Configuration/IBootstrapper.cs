using RoomScout.Presentation.Dispatcher;

namespace RoomScout.Presentation.Configuration
{
    public interface IBootstrapper
    {
        Task<ICommandDispatcher> BuildAsync(string hotelsPath, string bookingsPath);
    }
}
