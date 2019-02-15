using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DNetDebug.TypeReaders
{
    public class ExtendedUserTypeReader : UserTypeReader<IUser>
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input,
            IServiceProvider services)
        {
            var result = await base.ReadAsync(context, input, services).ConfigureAwait(false);
            if (result.IsSuccess) return result;
            var restClient = ((DiscordSocketClient)context.Client).Rest;
            if (restClient == null || !ulong.TryParse(input, out var userId)) return result;
            if (context.Guild != null)
            {
                var guildUser = await restClient.GetGuildUserAsync(context.Guild.Id, userId).ConfigureAwait(false);
                if (guildUser != null) return TypeReaderResult.FromSuccess(guildUser);
            }

            var user = await restClient.GetUserAsync(userId).ConfigureAwait(false);
            return user != null ? TypeReaderResult.FromSuccess(user) : result;
        }
    }
}