using DarkBot.src.Common;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.Logs
{
    public class JoinLeaveLogs
    {
        public static async Task UserJoin(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {

            if (!e.Member.IsBot)
            {
                var role = e.Guild.GetRole(1222923387937226875);

                if (role != null)
                {
                    await e.Member.GrantRoleAsync(role);
                }
            }
            var welcomeChannel = e.Guild.GetDefaultChannel();

            var welcomeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Magenta,
                Title = $"Welcome {e.Member.Nickname} !",
                Description = $"{e.Member.Mention}, enjoy your stay here. Check out <#1221919932372095178> to claim some Roles!",
                Timestamp = DateTimeOffset.Now,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = e.Guild.Name
                },
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = e.Member.AvatarUrl,
                    Name = e.Member.Username
                }
            };

            await welcomeChannel.SendMessageAsync(embed: welcomeEmbed);
        }

        public static async Task UserLeave(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberRemoveEventArgs e)
        {
            var logChannel = e.Guild.GetChannel(978346565418770433);

            var welcomeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Magenta,
                Title = $"Bye {e.Member.Nickname} !",
                Description = $"{e.Member.Mention} left the Discord!",
                Timestamp = DateTimeOffset.Now,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = e.Guild.Name
                },
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = e.Member.AvatarUrl,
                    Name = e.Member.Username
                }
            };

            await logChannel.SendMessageAsync(embed: welcomeEmbed);
        }
    }
}
