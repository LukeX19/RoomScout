using RoomScout.Business.Interfaces;

namespace RoomScout.Presentation.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<string, ICommandHandler> _handlers;

        public CommandDispatcher()
        {
            _handlers = new Dictionary<string, ICommandHandler>(StringComparer.OrdinalIgnoreCase);
        }

        public void Register(string commandName, ICommandHandler handler)
        {
            _handlers[commandName] = handler;
        }

        public async Task<string> DispatchAsync(string input)
        {
            foreach (var kvp in _handlers)
            {
                if (input.StartsWith($"{kvp.Key}(", StringComparison.OrdinalIgnoreCase))
                {
                    return await kvp.Value.ExecuteAsync(input);
                }
            }

            return "Unsupported command. Only Availability(...) and Search(...) are supported right now.";
        }
    }
}
