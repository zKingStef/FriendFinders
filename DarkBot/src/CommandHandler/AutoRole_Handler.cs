using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.CommandHandler
{
    public class AutoRole_Handler
    {
        public static async void GiveRoleToUser(ComponentInteractionCreateEventArgs e, string rolename)
        {
            var role = e.Guild.GetRole(1234567890);
            ulong roleid = 1234567890;

            switch (rolename)
            {
                case "dd_RolePokemonGo":
                    roleid = 1221805367466528908; // Pokemon Go
                    role = e.Guild.GetRole(roleid);
                    break;
                case "dd_RoleDarkServices":
                    roleid = 1222923387937226875; // DarkSolutions
                    role = e.Guild.GetRole(roleid);
                    break;
                //case "dd_RoleGamer":
                //    roleid = 978346565209042986;
                //    role = e.Guild.GetRole(roleid);
                //    break;
            }

            var member = await e.Guild.GetMemberAsync(e.User.Id);

            if (!member.Roles.Contains(role))
            {
                await member.GrantRoleAsync(role);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Role <@&{roleid}> granted")).AsEphemeral(true));
            }
            else
            {
                await member.RevokeRoleAsync(role);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                        .WithContent(($"Role <@&{roleid}> revoked")).AsEphemeral(true));
            }
        }
    }
}
