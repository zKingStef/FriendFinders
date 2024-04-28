using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.SlashCommands
{
	public class Troll_SL : ApplicationCommandModule
	{
        [SlashCommand("pingspam", "Spam Ping any User")]
        public static async Task PingSpam(InteractionContext ctx,
                            [Option("User", "Target User", autocomplete: false)] DiscordUser user,
                            [Option("Amount", "Amount of the Pings", autocomplete: false)] double amtPing)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                     .WithContent(($"PingSpam started...")).AsEphemeral(true));

            if (amtPing > 100)
            {
                await ctx.Channel.SendMessageAsync("Bro chill, why would you do this");
                return;
            }

            for (int i = 0; i < amtPing; i++)
                await ctx.Channel.SendMessageAsync(user.Mention);
        }
    }
}
