using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkBot.src.Common;
using DarkBot.src.CommandHandler;

namespace DarkBot.src.SlashCommands
{
	public class Moderation_SL : ApplicationCommandModule
	{
        [SlashCommand("clear", "Delete messages from the chat")]
        public  async Task Clear(InteractionContext ctx,
                               [Option("amount", "Amount of to be deleted messages")] double delNumber)
        {
            if (!CmdShortener.CheckPermissions(ctx, Permissions.ManageMessages))
            {
                await CmdShortener.SendAsEphemeral(ctx, ":x: Insufficient permissions!");
                return;
            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                     .WithContent(($"The last {delNumber} Messages have been deleted!")).AsEphemeral(true));

            var messages = await ctx.Channel.GetMessagesAsync((int)(delNumber));

            var content = new StringBuilder();
            content.AppendLine("Deleted Messages:");
            foreach (var message in messages)
            {
                content.AppendLine($"{message.Author.Username} ({message.Author.Id}) - {message.Content}");
            }

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                var msg = await new DiscordMessageBuilder()
                    .AddFile("deleted_messages.txt", memoryStream)
                    .SendAsync(ctx.Guild.GetChannel(1143516841357086870));
            }

            await CmdShortener.SendNotification(ctx,
                                                 $"Nachrichten mit /clear gelöscht!",
                                                 $"Anzahl Nachrichten: **{delNumber}**\n\n" +
                                                 $"Channel: {ctx.Channel.Mention} - {ctx.Channel.Name}\n" +
                                                 $"Verantwortlicher Moderator: {ctx.User.Mention}",
                                                 DiscordColor.Yellow,
                                                 1143516841357086870);

            await ctx.Channel.DeleteMessagesAsync(messages);
        }

        [SlashCommand("ban", "Ban a user from the Discord")]
        public  async Task Ban(InteractionContext ctx,
                          [Option("User", "User")] DiscordUser user,
                          [Option("Reason", "Ban Reason")] string reason,
                          [Option("AmountDays", "Delete all Messages that have been sent by the User")] double deleteDays = 0)
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ExecuteBanAction(ctx, (DiscordMember)user, (int)deleteDays, reason);
        }

        [SlashCommand("banid", "Ban a user from the Discord")]
        [RequireBotPermissions(DSharpPlus.Permissions.Administrator, true)]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator, true)]
        public  async Task BanId(InteractionContext ctx,
                          [Option("UserID", "ID of the User")] string userId,
                          [Option("Grund", "Ban Reason")] string reason,
                          [Option("AnzahlTage", "Delete all Messages that have been sent by the User")] double deleteDays = 0)
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ExecuteBanAction(ctx, userId, (int)deleteDays, reason);
        }

        [SlashCommand("unban", "Unban a user from the Discord. Leave blank to open Banlist")]
        public  async Task UnbanOrListBans(InteractionContext ctx,
                                   [Option("UserId", "ID of the User")] string? userId = null,
                                   [Option("Grund", "Unban Reason")] string reason = "No Reason")
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ExecuteUnbanAction(ctx, userId, reason);

        }

        [SlashCommand("banlist", "Show all banned Users")]
        public  async Task Banlist(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ShowBanList(ctx);
        }

        [SlashCommand("mute", "Setze einen Timeout auf einen Benutzer.")]
        public  async Task Timeout(InteractionContext ctx,
                          [Option("User", "Der User, der einen Timeout erhalten soll")] DiscordUser user,
                          [Option("Duration", "Dauer des Timeouts in Minuten")] long durationInMinutes,
                          [Option("Reason", "Grund für den Timeout")] string reason = "Kein Grund angegeben")
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ExecuteTimeoutAction(ctx, (DiscordMember)user, durationInMinutes, reason);
        }

        [SlashCommand("unmute", "Hebt den Timeout eines Benutzers auf.")]
        public  async Task RemoveTimeout(InteractionContext ctx,
                                [Option("User", "Der User, dessen Timeout aufgehoben werden soll")] DiscordUser user,
                                [Option("Reason", "Grund für die Aufhebung des Timeouts")] string reason = "Kein Grund angegeben")
        {
            await ctx.DeferAsync();
            await Moderation_Handler.ExecuteRemoveTimeoutAction(ctx, (DiscordMember)user, reason);
        }

        [SlashCommand("lock", "Sperrt temporär den Schreibzugriff für alle User.")]
        public  async Task Lock(InteractionContext ctx)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            await ctx.DeferAsync();
            await ctx.Channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.None, Permissions.SendMessages);

            await CmdShortener.SendNotification(ctx, "Channel gesperrt!", "Bitte warte bis der Channel von einem Admin freigeschalten wird.", DiscordColor.HotPink, 0);
        }

        [SlashCommand("unlock", "Gibt einen Channel wieder frei.")]
        public  async Task Unlock(InteractionContext ctx)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            await ctx.DeferAsync();
            await ctx.Channel.AddOverwriteAsync(ctx.Guild.EveryoneRole, Permissions.SendMessages, Permissions.None);

            await CmdShortener.SendNotification(ctx, "Channel freigeschalten!", "Der Channel ist jetzt wieder offen. Danke für eure Geduld!", DiscordColor.HotPink, 0);
        }
    }
}
