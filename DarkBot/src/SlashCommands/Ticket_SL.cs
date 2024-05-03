using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using DarkBot.src.Handler;
using DarkBot.src.Common;
using DarkBot.src.CommandHandler;

namespace DarkBot.src.SlashCommands
{
    [SlashCommandGroup("ticket", "Alle Ticket Befehle")]
    public class Ticket_SL : ApplicationCommandModule
    {
        [SlashCommand("system", "Erschaffe das Ticket System")]
        public static async Task Ticketsystem(InteractionContext ctx,
                                [Choice("Valo", 0)]
                                [Choice("CS2", 1)]
                                [Option("form", "Welche Ticket Form?")] long systemChoice = 1)
        {
            // Pre Execution Checks
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            if (systemChoice == 0)
            {
                var embedTicketButtons = new DiscordEmbedBuilder()
                    .WithTitle("**Valorant Clan Beitrittsformular**")
                    .WithColor(DiscordColor.IndianRed)
                    .WithDescription("Fülle dieses Formular aus, um dich bei uns für den Valorant Clan zu bewerben.")
                    .WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Valorant_logo_-_pink_color_version.svg/1280px-Valorant_logo_-_pink_color_version.svg.png");

                var buttonComponent = new DiscordButtonComponent(ButtonStyle.Success, "ticketValoClanBtn", "📩 Zum Formular");

                var messageBuilder = new DiscordMessageBuilder()
                    .WithEmbed(embedTicketButtons)
                    .AddComponents(buttonComponent);

                await ctx.Channel.SendMessageAsync(messageBuilder);
            }

            else if (systemChoice == 1)
            {
                var embedTicketButtons = new DiscordEmbedBuilder()
                    .WithTitle("**CS2 Clan Beitrittsformular**")
                    .WithColor(DiscordColor.Orange)
                    .WithDescription("Fülle dieses Formular aus, um dich bei uns für den CS2 Clan zu bewerben.")
                    .WithImageUrl("https://www.memorypc.de/media/image/8d/5f/72/CS2banner_600x600.webp");

                var buttonComponent = new DiscordButtonComponent(ButtonStyle.Success, "ticketCS2ClanBtn", "📩 Zum Formular");

                var messageBuilder = new DiscordMessageBuilder()
                    .WithEmbed(embedTicketButtons)
                    .AddComponents(buttonComponent);

                await ctx.Channel.SendMessageAsync(messageBuilder);
            }
        }

        [SlashCommand("add", "Add a User to the Ticket")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task Add(InteractionContext ctx,
                             [Option("User", "The user which will be added to the ticket")] DiscordUser user)
        {
            // Pre Execution Checks
            await Ticket_Handler.CheckIfUserHasTicketPermissions(ctx);
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Ticket",
                Description = $"{user.Mention} wurde zum Ticket hinzugefügt von {ctx.User.Mention}!\n",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.AddOverwriteAsync((DiscordMember)user, Permissions.AccessChannels);
        }

        [SlashCommand("remove", "Remove a User from the Ticket")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task Remove(InteractionContext ctx,
                             [Option("User", "The user, which will be removed from the ticket")] DiscordUser user)
        {
            // Pre Execution Checks
            await Ticket_Handler.CheckIfUserHasTicketPermissions(ctx);
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Ticket",
                Description = $"{user.Mention} wurde aus dem Ticket entfernt von {ctx.User.Mention}!\n",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.AddOverwriteAsync((DiscordMember)user, Permissions.None);
        }

        [SlashCommand("rename", "Change the Name of the Ticket")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task Rename(InteractionContext ctx,
                             [Option("Name", "New Name of the Ticket")] string newChannelName)
        {
            // Pre Execution Checks
            await Ticket_Handler.CheckIfUserHasTicketPermissions(ctx);
            await CheckIfChannelIsTicket(ctx);

            var oldChannelName = ctx.Channel.Mention;

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "Ticket",
                Description = $"Ticket {ctx.Channel.Mention} wurde umbenannt von {ctx.User.Mention}!\n\n" +
                              $"Neuer Ticketname: ```{newChannelName}```",
                Timestamp = DateTime.UtcNow
            };

            await ctx.CreateResponseAsync(embedMessage);

            await ctx.Channel.ModifyAsync(properties => properties.Name = newChannelName);
        }

        [SlashCommand("close", "Close a Ticket")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task Close(InteractionContext ctx)
        {
            // Pre Execution Checks
            await Ticket_Handler.CheckIfUserHasTicketPermissions(ctx);
            await CheckIfChannelIsTicket(ctx);

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "🔒 Ticket geschlossen!",
                Description = $"Das Ticket wurde von {ctx.User.Mention} geschlossen!\n" +
                              $"Der Kanal wird in <t:{DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds()}:R> gelöscht.",
                Timestamp = DateTime.UtcNow
            };
            await ctx.CreateResponseAsync(embedMessage);

            var messages = await ctx.Channel.GetMessagesAsync(999);

            var content = new StringBuilder();
            content.AppendLine($"Transcript Ticket {ctx.Channel.Name}:");
            foreach (var message in messages)
            {
                content.AppendLine($"{message.Author.Username} ({message.Author.Id}) - {message.Content}");
            }

            await Task.Delay(TimeSpan.FromSeconds(60));

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                var msg = await new DiscordMessageBuilder()
                    .AddFile("ticketLog.txt", memoryStream)
                    .SendAsync(ctx.Guild.GetChannel(1209297588915015730));
            }

            await ctx.Channel.DeleteAsync("Ticket geschlossen");
        }

        private async Task<bool> CheckIfChannelIsTicket(InteractionContext ctx)
        {
            const ulong categoryId = 1197912790208356422;

            if (ctx.Channel.Parent.Id != categoryId || ctx.Channel.Parent == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent(":warning: **Dieser Befehl ist nur für Tickets geeignet!**").AsEphemeral(true));

                return true;
            }

            return false;
        }
    }
}
