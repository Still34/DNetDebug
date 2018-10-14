using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
    }
}