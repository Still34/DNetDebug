using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DNetDebug.Helpers;
using DNetDebug.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DNetDebug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Casino was here, 2018");

            using (var services = ConfigureService())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var config = services.GetRequiredService<IConfigurationRoot>();
                client.Log += LoggingHelper.LogAsync;

                services.GetRequiredService<InspectService>();
                await services.GetRequiredService<CommandHandler>().SetupAsync().ConfigureAwait(false);
                await client.LoginAsync(TokenType.Bot, config["DiscordDebugToken"]).ConfigureAwait(false);
                await client.StartAsync().ConfigureAwait(false);
                await Task.Delay(-1).ConfigureAwait(false);
            }
        }

        internal ServiceProvider ConfigureService()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            return new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<InspectService>()
                .AddSingleton<CommandService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
