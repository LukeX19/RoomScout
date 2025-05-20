using RoomScout.Business.Interfaces;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Empty command.";
            }

            input = input.Trim();

            foreach (var pair in _handlers)
            {
                // Regex: optional spaces
                var pattern = $@"^{pair.Key}\s*\(";
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return await pair.Value.ExecuteAsync(input);
                }
            }

            return "Unsupported command. Only Availability(...) and Search(...) are supported right now.";
        }
    }
}
