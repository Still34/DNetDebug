using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DNetDebug.Helpers;
using DNetDebug.Results;
using DNetDebug.Services;
using DNetDebug.TypeReaders;

namespace DNetDebug.Modules
{
    [Group("inspect")]
    public class InspectModule : ModuleBase<SocketCommandContext>
    {
        private readonly InspectService _inspect;

        public InspectModule(InspectService inspect) => _inspect = inspect;

        [Command("message")]
        public Task InspectMessageAsync(IUserMessage msg = null)
        {
            if (msg == null) msg = Context.Message;
            return ReplyAsync(msg.Inspect());
        }

        [Command("embed")]
        public async Task InspectEmbedAsync(IUserMessage msg = null)
        {
            if (msg == null) msg = Context.Message;
            if (!msg.Embeds.Any()) await ReplyAsync("This message does not have an embed.").ConfigureAwait(false);
            foreach (var msgEmbed in msg.Embeds) await ReplyAsync(msgEmbed.Inspect()).ConfigureAwait(false);
        }

        [Command("guild")]
        public Task GetGuildAsync(ulong id = 0)
        {
            var guild = id == 0 ? Context.Guild : Context.Client.GetGuild(id);
            return guild == null
                ? ReplyAsync("This guild does not exist or is not accessible.")
                : ReplyAsync(guild.Inspect());
        }

        [Command("user")]
        public Task GetUserAsync([OverrideTypeReader(typeof(ExtendedUserTypeReader))]
            IUser user = null)
            => user == null
                ? ReplyAsync(Context.User.Inspect())
                : ReplyAsync(user.Inspect());

        [Command("start")]
        public Task<RuntimeResult> StartServiceAsync()
        {
            if (_inspect.IsEnabled)
                return Task.FromResult<RuntimeResult>(
                    CommonResult.FromError("Inspection service had already been enabled."));

            _inspect.Start();
            return Task.FromResult<RuntimeResult>(
                CommonResult.FromSuccess("Inspection service has started!" +
                                         Environment.NewLine +
                                         "All messages from now on will be inspected."));
        }

        [Command("stop")]
        public Task<RuntimeResult> StopServiceAsync()
        {
            if (!_inspect.IsEnabled)
                return Task.FromResult<RuntimeResult>(
                    CommonResult.FromError("Inspection service had already been disabled."));

            _inspect.Stop();
            return Task.FromResult<RuntimeResult>(
                CommonResult.FromSuccess("Inspection service has stopped!" +
                                         Environment.NewLine +
                                         "All messages from now on will NOT be inspected."));
        }
    }
}