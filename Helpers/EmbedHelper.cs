using Discord;

namespace DNetDebug.Helpers
{
    public class EmbedHelper
    {
        public static Embed FromInfo(string title = null, string description = null, 
            EmbedFooterBuilder footer = null)
            => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.imgur.com/BGx359P.png",
                    Name = title ?? "Information"
                },
                Description = description,
                Color = Color.Blue,
                Footer = footer
            }.Build();

        public static Embed FromWarning(string title = null, string description = null,
            EmbedFooterBuilder footer = null)
            => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.imgur.com/euWbiQP.png",
                    Name = title ?? "Warning"
                },
                Description = description,
                Color = Color.Orange,
                Footer = footer
            }.Build();

        public static Embed FromSuccess(string title = null, string description = null,
            EmbedFooterBuilder footer = null)
            => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.imgur.com/Qlr5oRK.png",
                    Name = title ?? "Success"
                },
                Description = description,
                Color = Color.Green,
                Footer = footer
            }.Build();

        public static Embed FromError(string title = null, string description = null, 
            EmbedFooterBuilder footer = null)
            => new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "https://i.imgur.com/eOoFlam.png",
                    Name = title ?? "Error"
                },
                Description = description,
                Color = Color.Red,
                Footer = footer
            }.Build();
    }
}