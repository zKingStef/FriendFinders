using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkBot.src.CommandHandler;

namespace DarkBot.src.SlashCommands
{
    public class ImgFinder_SL : ApplicationCommandModule
    {
        [SlashCommand("googlebild", "Google Bild Search")]
        public async Task GoogleImageSearch(InteractionContext ctx,
                                            [Option("search", "Nach welchem Bild möchtest du suchen ? ")] string search)
        {
            var imageUrl = await ImgFinder_Handler.GetImageUrl(search);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Bildsuchergebnis für: " + search)
                    .WithImageUrl(imageUrl)
                    .WithColor(DiscordColor.Gold);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(embed));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("No Pictures found."));
            }
        }

        [SlashCommand("dog", "Generate a random dog picture")]
        public static async Task DogImage(InteractionContext ctx)
        {
            var dog = "http://random.dog/" + await ImgFinder_Handler.GetResponseStringAsync("https://random.dog/woof").ConfigureAwait(false);
            var embed = new DiscordEmbedBuilder().WithImageUrl(dog).WithTitle("Dog").WithUrl(dog);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed.Build()));

        }
    }
}
