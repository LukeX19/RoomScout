namespace RoomScout.Presentation.Dispatcher
{
    public interface ICommandDispatcher
    {
        Task<string> DispatchAsync(string input);
    }
}
