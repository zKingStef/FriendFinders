using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using DarkBot.src.Common;

namespace DarkBot.src.SlashCommands
{
    public class Misc_SL : ApplicationCommandModule
    {
        [SlashCommand("valonewbie", "Vergib die Valo Newbie Rolle")]
        public static async Task ValoNewbieRole(InteractionContext ctx,
                                       [Option("User", "User")] DiscordUser user)
        {
            ulong roleid = 1183220649825685675;
            var role = ctx.Guild.GetRole(roleid);

            if (!CmdShortener.CheckRole(ctx, roleid))
            {
                await CmdShortener.SendNotification(ctx, "Keine Rechte", "Du benötigst die Bereichsleiter Rolle für diesen Befehl!", DiscordColor.Red, 0);
                return;
            }

            var member = await ctx.Guild.GetMemberAsync(user.Id);

            if (!member.Roles.Contains(role))
            {
                await member.GrantRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Der Spieler hat die Rolle <@&{roleid}> erhalten.")).AsEphemeral(true));
            }
            else
            {
                await member.RevokeRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Du hast dem Spieler die Rolle <@&{roleid}> entfernt.")).AsEphemeral(true));
            }
        }

        [SlashCommand("cs2newbie", "Vergib die Valo Newbie Rolle")]
        public static async Task CS2NewbieRole(InteractionContext ctx,
                                       [Option("User", "User")] DiscordUser user)
        {
            ulong roleid = 1220450541511905290;
            var role = ctx.Guild.GetRole(roleid);

            if (!CmdShortener.CheckRole(ctx, roleid))
            {
                await CmdShortener.SendNotification(ctx, "Keine Rechte", "Du benötigst die Bereichsleiter Rolle für diesen Befehl!", DiscordColor.Red, 0);
                return;
            }

            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);

            if (!member.Roles.Contains(role))
            {
                await member.GrantRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Der Spieler hat die Rolle <@&{roleid}> erhalten.")).AsEphemeral(true));
            }
            else
            {
                await member.RevokeRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Du hast dem Spieler die Rolle <@&{roleid}> entfernt.")).AsEphemeral(true));
            }
        }
    }
}
