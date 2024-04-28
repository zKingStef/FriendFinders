using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DarkBot.src.Common;
using DSharpPlus.SlashCommands;

namespace DarkBot.src.CommandHandler
{
    public class Ticket_Handler
    {
        public static async void HandlePoGoTickets(ComponentInteractionCreateEventArgs e, string ttype)
        {
            DiscordMember? user = e.User as DiscordMember;
            DiscordGuild guild = e.Guild;

            if (guild.GetChannel(1207086767623381092) is not DiscordChannel category || category.Type != ChannelType.Category)
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Fehler beim Erstellen des Tickets: Eine Kategorie für Tickets konnte nicht gefunden werden.").AsEphemeral(true));
                return;
            }

            var overwrites = new List<DiscordOverwriteBuilder>
                {
                    new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                    new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                };

            DiscordChannel channel = await guild.CreateTextChannelAsync($"{e.User.Username}-Ticket", category, overwrites: overwrites, position: 0);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Dein neues Ticket ({channel.Mention}) wurde erstellt!").AsEphemeral(true));

            var closeButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeTicketButton", "🔒 Close Ticket");

            _ = await channel.SendMessageAsync($"||{user.Mention}||");

            string ticketDesc = "ERROR";
            string ticketTitle = "ERROR";
            var embedColor = DiscordColor.White;

            switch (ttype)
            {
                case "dd_TicketPokecoins":
                    ticketDesc = "**Attention:** Please tell me the Amount of Pokecoins you want to order and what payment method you will be using.";
                    ticketTitle = "Pokecoin";
                    embedColor = DiscordColor.Yellow;
                    break;
                case "dd_TicketStardust":
                    ticketDesc = "**[!]** Please tell me the Amount of **Stardust** you want to order and what payment method you will be using.\n";
                    ticketTitle = "Stardust";
                    embedColor = DiscordColor.HotPink;
                    break;
                case "dd_TicketXp":
                    ticketDesc = "**[!]** Please tell me if you want the 5 Hour XP Service or if you are looking for something else ?";
                    ticketTitle = "XP";
                    embedColor = DiscordColor.Blue;
                    break;
            }

            var ticketMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(embedColor)
                    .WithTitle("__" + ticketTitle + " Service__")
                    .WithThumbnail(guild.IconUrl)
                    .WithDescription("**Thank you for opening a Ticket!\n\n" +
                                     "<@293068127049089024> will respond as soon as possible.**\n\n" + ticketDesc)
                    )
                    .AddComponents(closeButton);
            await channel.SendMessageAsync(ticketMessage);
        }

        public static async void HandleGeneralTickets(ComponentInteractionCreateEventArgs e, string ttype)
        {
            DiscordMember? user = e.User as DiscordMember;
            DiscordGuild guild = e.Guild;

            if (guild.GetChannel(1207086767623381092) is not DiscordChannel category || category.Type != ChannelType.Category)
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Fehler beim Erstellen des Tickets: Eine Kategorie für Tickets konnte nicht gefunden werden.").AsEphemeral(true));
                return;
            }

            var overwrites = new List<DiscordOverwriteBuilder>
                {
                    new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                    new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                };

            DiscordChannel channel = await guild.CreateTextChannelAsync($"{e.User.Username}-Ticket", category, overwrites: overwrites, position: 0);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ticket created: ({channel.Mention})").AsEphemeral(true));

            var closeButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeTicketButton", "🔒 Ticket schließen");

            await channel.SendMessageAsync($"||{user.Mention}||");

            string ticketDesc = "ERROR";
            string ticketTitle = "ERROR";

            switch (ttype)
            {
                case "dd_TicketDarkSolutions":
                    ticketDesc = "Please give us following Information:, " +
                                 "- Which Service are you interested in ?\n What Payment Method will you be using ?";
                    ticketTitle = "Dark Services";
                    break;
                case "dd_TicketSupport":
                    ticketDesc = "**Beachte:** Bitte beschreibe dein Problem mit ein paar Worten, " +
                                 "damit wir schnellstmöglich auf dein Ticket reagieren können, um " +
                                 "dein Anliegen schnellstmöglich zu lösen.";
                    ticketTitle = "Support Ticket";
                    break;
                case "dd_TicketUnban":
                    ticketDesc = "Bitte schreib folgende Informationen ins Ticket:\n" +
                                 "- Ingame Namen: \n" +
                                 "- Ban ID: \n" +
                                 "- Grund für die Entbannung";
                    ticketTitle = "Entbannungsantrag";
                    break;
                case "dd_TicketDonation":
                    ticketDesc = "**Spenden sind freiwillig und keinesfalls Pflicht.**";
                    ticketTitle = "Spende";
                    break;
                case "dd_TicketOwner":
                    ticketDesc = "**Beachte:** Dieses Ticket geht direkt an den Inhaber und kann auch nur von ihm bearbeitet werden. " +
                                 "Eine höhere Wartzeit als bei normalen Tickets ist zu erwarten.";
                    ticketTitle = "Inhaber Anfrage";
                    break;
                case "dd_TicketApplication":
                    ticketDesc = "**Beachte:** Bitte schreib erstmal ein paar Worte zu deiner Person " +
                                 "(Wer bist du ? Wie alt ?) und als was du dich bewerben möchtest?\n";
                    ticketTitle = "Teambewerbung";
                    break;
            }

            var ticketMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Orange)
                    .WithTitle("__" + ticketTitle + "__")
                    .WithThumbnail(guild.IconUrl)
                    .WithDescription("**In Kürze wird sich jemand um dich kümmern!**\n" +
                                     "Sollte dein Anliegen bereits erledigt sein dann drücke auf 🔒 um dein Ticket zu schließen!\n\n" + ticketDesc)
                    )
                    .AddComponents(closeButton);
            await channel.SendMessageAsync(ticketMessage);
        }

        public static async Task CheckIfUserHasTicketPermissions(InteractionContext ctx)
        {
            if (!CmdShortener.CheckRole(ctx, 978352059617280010))
            {
                await CmdShortener.SendNotification(ctx, "No access", "You do not have the necessary permissions to execute this command.", DiscordColor.Red, 0);
                return;
            }
        }
        
}
}
