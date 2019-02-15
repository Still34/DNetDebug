using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DNetDebug.Modules
{
    public class DebugModule : ModuleBase<SocketCommandContext>
    {
        [Command("emoji")]
        public Task ReactWithEmoji(string rawEmoji)
        {
            var emoji = new Emoji(rawEmoji);
            return Context.Message.AddReactionAsync(emoji);
        }

        [Command("msg")]
        public async Task GetMessageFromBeginningAsync(SocketTextChannel channel, int msgCount)
        {
            var messages = await channel.GetMessagesAsync(fromMessageId: 0, dir: Direction.After, limit: msgCount).FlattenAsync();
            await ReplyAsync(string.Join(",\n", messages.Select(x => x.Content + ", "+ x.CreatedAt)));
        }
    }
}