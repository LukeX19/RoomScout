namespace RoomScout.Business.Interfaces
{
    public interface ICommandHandler
    {
        Task<string> ExecuteAsync(string rawCommand);
    }
}
