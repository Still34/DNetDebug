using System.Threading.Tasks;
using Discord.WebSocket;
using DNetDebug.Helpers;

namespace DNetDebug.Services
{
    public class InspectService
    {
        private readonly DiscordSocketClient _client;
        public bool IsEnabled { get; private set; }

        public InspectService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Start()
        {
            _client.MessageReceived += PrintMessagePropertiesAsync;
            IsEnabled = true;
        }

        public void Stop()
        {
            _client.MessageReceived -= PrintMessagePropertiesAsync;
            IsEnabled = false;
        }

        private Task PrintMessagePropertiesAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage userMsg) ||
                userMsg.Author.Id == _client.CurrentUser.Id) return Task.CompletedTask;
            var inspectResult = userMsg.Inspect();
            return userMsg.Channel.SendMessageAsync(inspectResult);
        }
    }
}