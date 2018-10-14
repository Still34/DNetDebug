using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNetDebug.Helpers;
using DNetDebug.Results;
using Microsoft.Extensions.DependencyInjection;

namespace DNetDebug
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _command;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _command = _services.GetRequiredService<CommandService>();
        }

        public async Task SetupAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            _command.CommandExecuted += HandlePostCommandExecutionAsync;
            _command.Log += LoggingHelper.LogAsync;
            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services).ConfigureAwait(false);
        }

        private async Task HandlePostCommandExecutionAsync(CommandInfo commandInfo, ICommandContext context,
            IResult result)
        {
            if (result.Error == CommandError.UnknownCommand) return;
            if (!string.IsNullOrEmpty(result.ErrorReason))
            {
                Embed embed;
                switch (result)
                {
                    case CommonResult commonResult:
                        switch (commonResult.CommonResultType)
                        {
                            default:
                                embed = EmbedHelper.FromInfo(description: commonResult.Reason);
                                break;
                            case CommonResultType.Warning:
                                embed = EmbedHelper.FromWarning(description: commonResult.Reason);
                                break;
                            case CommonResultType.Error:
                                embed = EmbedHelper.FromError(description: commonResult.Reason);
                                break;
                            case CommonResultType.Success:
                                embed = EmbedHelper.FromSuccess(description: commonResult.Reason);
                                break;
                        }

                        break;
                    default:
                        embed = EmbedHelper.FromError(description: result.ErrorReason);
                        break;
                }

                var message = await context.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
                if (result.Error.HasValue)
                    _ = Task.Delay(TimeSpan.FromSeconds(5))
                        .ContinueWith(_ => message?.DeleteAsync().ConfigureAwait(false));
            }
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage userMsg)) return;
            if (userMsg.Source != MessageSource.User) return;
            var argPos = 0;
            if (!userMsg.HasCharPrefix('!', ref argPos)) return;
            var context = new SocketCommandContext(_client, userMsg);
            var result = await _command.ExecuteAsync(context, argPos, _services).ConfigureAwait(false);
            await HandlePostCommandExecutionAsync(null, context, result).ConfigureAwait(false);
        }
    }
}