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
    public class InviteLogs
    {
        public static async Task InviteDeleted(DiscordClient sender, DSharpPlus.EventArgs.InviteDeleteEventArgs e)
        {
            await CmdShortener.SendLogMessage(sender,
                                            1143517864859549776,
                                            AuditLogActionType.InviteDelete,
                                            "Invite deleted!",
                                            $"**User:** {e.Invite.Inviter.Mention} \n" +
                                            $"**Invite:** {e.Invite.Code} \n" +
                                            $"**Times used**: {e.Invite.Uses}\n" +
                                            $"**max. Uses:** {e.Invite.MaxUses}\n",
                                            DiscordColor.IndianRed);
        }

        public static async Task InviteCreated(DiscordClient sender, DSharpPlus.EventArgs.InviteCreateEventArgs e)
        {
            await CmdShortener.SendLogMessage(sender,
                                            1143517864859549776,
                                            "Invite Link created!",
                                            $"**User:** {e.Invite.Inviter.Mention} \n" +
                                            $"**Invite:** {e.Invite.Code}, **expires** {e.Invite.ExpiresAt} \n" +
                                            $"**max. Uses:** {e.Invite.MaxUses} \n",
                                            DiscordColor.Orange);
        }
    }
}
