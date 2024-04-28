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
    [SlashCommandGroup("ticket", "Slash Commands for the Ticketsystem.")]
    public class Ticket_SL : ApplicationCommandModule
    {
        [SlashCommand("system", "Summon Ticket System")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public  async Task Ticketsystem(InteractionContext ctx,
                                [Choice("Button", 0)]
                                [Choice("Dropdown Menu", 1)]
                                [Option("system", "Buttons oder Dropdown")] long systemChoice = 1)
        {
            // Pre Execution Checks
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            if (systemChoice == 0)
            {
                var embedTicketButtons = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.White)
                .WithTitle("**Ticket-System**")
                .WithDescription("Klicke auf einen Button, um ein Ticket der jeweiligen Kategorie zu erstellen")
                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Success, "ticketDarkServiceBtn", "Support"),
                    //new DiscordButtonComponent(ButtonStyle.Success, "ticketSupportBtn", "Support"),
                    //new DiscordButtonComponent(ButtonStyle.Danger, "ticketUnbanBtn", "Entbannung"),
                    //new DiscordButtonComponent(ButtonStyle.Primary, "ticketDonationBtn", "Spenden"),
                    //new DiscordButtonComponent(ButtonStyle.Secondary, "ticketOwnerBtn", "Inhaber"),
                    //new DiscordButtonComponent(ButtonStyle.Success, "ticketApplyBtn", "Bewerben")
                });

                var response = new DiscordInteractionResponseBuilder().AddEmbed(embedTicketButtons.Embeds[0]).AddComponents(embedTicketButtons.Components);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
            }

            else if (systemChoice == 1)
            {
                var dropdownComponents = new List<DiscordSelectComponentOption>()
                {
                    new(
                        "Dark Solutions", "dd_TicketDarkSolutions", "Order any Service here!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":DarkServices:"))),

                    //new(
                    //    "Support", "dd_TicketSupport", "Allgemeine Probleme, Fragen, Wünsche und sonstiges!",
                    //    emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":envelope:"))),
                    //
                    //new(
                    //    "Entbannung", "dd_TicketUnban", "Duskutiere über einen Bann!",
                    //    emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":tickets:"))),
                    //
                    //new(
                    //    "Spenden", "dd_TicketDonation", "Ticket für Donations!",
                    //    emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":moneybag:"))),
                    //
                    //new(
                    //    "Inhaber", "dd_TicketOwner", "Dieses Ticket geht speziell an den Inhaber des Servers!",
                    //    emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":factory_worker:"))),
                    //
                    //new(
                    //    "Bewerben", "dd_TicketApplication", "Bewerbung für das Team!",
                    //    emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":writing_hand:")))
                };

                var ticketDropdown = new DiscordSelectComponent("ticketDropdown", "Open your Ticket here...", dropdownComponents, false, 0, 1);

                var embedTicketDropdown = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()

                    .WithColor(DiscordColor.Cyan)
                    .WithTitle("**DarkSolutions Ticket-System**")
                    .WithDescription("Open the Dropdown Menu and click on the Category you want to create a Ticket of")
                    )
                    .AddComponents(ticketDropdown);

                await CmdShortener.SendAsEphemeral(ctx, "Ticketsystem successfully loaded.");

                await ctx.Channel.SendMessageAsync(embedTicketDropdown);
            }
        }
        
        [SlashCommand("pogosystem", "Erschaffe das Ticketsystem für Pokemon Go")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task TicketsystemPOGO(InteractionContext ctx)
        {
            // Pre Execution Checks
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            var dropdownComponents = new List<DiscordSelectComponentOption>()
                {
                    new(
                        "Pokecoins", "dd_TicketPokecoins", "Order a Pokecoin Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Pokecoin1:"))),

                    new(
                        "Stardust", "dd_TicketStardust", "Order a Stardust Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Stardust:"))),

                    new(
                        "XP", "dd_TicketXp", "Order a XP Service!",
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(ctx.Client, ":Level40:")))
                };

            var ticketDropdown = new DiscordSelectComponent("ticketDropdown", "Choose a Ticket", dropdownComponents, false, 0, 1);

            var embedTicketDropdown = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.IndianRed)
                .WithTitle("Open Ticket To Buy Service")
                .WithDescription("Feel free to open a ticket if you want to know more about the services or if you want to order any Service.\n\n" +
                                 "All my Services are completely safe for your account. 3 Years no Ban/Strike")
                )
                .AddComponents(ticketDropdown);

            await CmdShortener.SendAsEphemeral(ctx, "Ticketsystem successfully loaded.");

            await ctx.Channel.SendMessageAsync(embedTicketDropdown);
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
                    new DiscordInteractionResponseBuilder().WithContent(":warning: **This Command is for Tickets only!**").AsEphemeral(true));

                return true;
            }

            return false;
        }
    }
}
