using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.PrefixCommands
{
    public class Misc_PX : BaseCommandModule
    {
        [Command("sendmessage")]
        public static async Task SendMessage(CommandContext ctx, [RemainingText] string message)
        {
            // Sende die Nachricht im aktuellen Kanal
            await ctx.RespondAsync(message);
        }
    }
}
