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
        public  async Task Ticketsystem(InteractionContext ctx,
                                [Choice("Valo", 0)]
                                [Choice("CS2", 1)]
                                [Option("form", "Welche Ticket Form?")] long systemChoice = 1)
        {
            // Pre Execution Checks
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            if (systemChoice == 0)
            {
                var embedTicketButtons = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("**Valorant Clan Beitrittsformular**")
                        .WithColor(DiscordColor.IndianRed)
                        .WithDescription("Fülle dieses Formular aus, um dich bei uns für den Valorant Clan zu bewerben.")
                        .WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Valorant_logo_-_pink_color_version.svg/1280px-Valorant_logo_-_pink_color_version.svg.png")
                    )
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Success, "ticketValoClanBtn", "Zum Formular")
                    });

                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embedTicketButtons.Embeds[0])
                    .AddComponents(embedTicketButtons.Components);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            }

            else if (systemChoice == 1)
            {
                var embedTicketButtons = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("**CS2 Clan Beitrittsformular**")
                        .WithColor(DiscordColor.Orange)
                        .WithDescription("Fülle dieses Formular aus, um dich bei uns für den CS2 Clan zu bewerben.")
                        .WithImageUrl("https://www.memorypc.de/media/image/8d/5f/72/CS2banner_600x600.webp")
                    )
                    .AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Success, "ticketCS2Btn", "Zum Formular")
                    });

                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embedTicketButtons.Embeds[0])
                    .AddComponents(embedTicketButtons.Components);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
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
                Title = "User added!",
                Description = $"{user.Mention} has been added to the Ticket by {ctx.User.Mention}!\n",
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
                Title = "User removed!",
                Description = $"{user.Mention} has been removed from the Ticket by {ctx.User.Mention}!\n",
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
                Title = "Ticket renamed!",
                Description = $"The Ticket {ctx.Channel.Mention} has been renamed by {ctx.User.Mention}!\n\n" +
                              $"New Ticket Name: ```{newChannelName}```",
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
                Title = "🔒 Ticket closed!",
                Description = $"The Ticket has been closed by {ctx.User.Mention}!\n" +
                              $"The Channel will be deleted in <t:{DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds()}:R>.",
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
                    .AddFile("transript.txt", memoryStream)
                    .SendAsync(ctx.Guild.GetChannel(978669571483500574));
            }

            await ctx.Channel.DeleteAsync("Ticket closed");
        }

        private async Task<bool> CheckIfChannelIsTicket(InteractionContext ctx)
        {
            const ulong categoryId = 1207086767623381092;

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
