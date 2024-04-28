using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkBot.src.Common;

namespace DarkBot.src.Logs
{
    public class UnBanLogs
    {
        public static async Task UserUnbanned(DiscordClient sender, GuildBanRemoveEventArgs e)
        {
            await CmdShortener.SendLogMessage(sender,
                                               1143518613777678336,
                                               AuditLogActionType.Unban,
                                               "User unbanned!",
                                               $"**User:** {e.Member.Mention}- {e.Member.DisplayName}\n " +
                                               $"DiscordId: {e.Member.Id}\n\n" +
                                               "**Moderator:** ",
                                               DiscordColor.SpringGreen);
        }

        public static async Task UserBanned(DiscordClient sender, GuildBanAddEventArgs e)
        {
            await CmdShortener.SendLogMessage(sender,
                                               1143518462111658034,
                                               AuditLogActionType.Ban,
                                               "User banned!",
                                               $"**User:** {e.Member.Mention} - {e.Member.DisplayName} \n " +
                                               $"DiscordId: {e.Member.Id}\n\n" +
                                               "**Moderator:** ",
                                               DiscordColor.IndianRed);
        }
    }
}
