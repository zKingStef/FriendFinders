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
        public static async void HandleGeneralTickets(ComponentInteractionCreateEventArgs e)
        {
            DiscordMember? user = e.User as DiscordMember;
            DiscordGuild guild = e.Guild;

            if (guild.GetChannel(1197912790208356422) is not DiscordChannel category || category.Type != ChannelType.Category)
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Fehler beim Erstellen des Tickets: Eine Kategorie für Tickets konnte nicht gefunden werden.").AsEphemeral(true));
                return;
            }

            string ticketDesc = "ERROR";
            string ticketTitle = "ERROR";
            var overwrites = new List<DiscordOverwriteBuilder>
            {
                new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
            };

            switch (e.Interaction.Data.CustomId)
            {
                case "ticketValoClanBtn":
                    ticketDesc = "Tdwdwd";
                    ticketTitle = "Valorant Clan Bewerbung";

                    overwrites = new List<DiscordOverwriteBuilder>
                    {
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(guild.GetRole(1220804206269567087)).Allow(Permissions.AccessChannels), // Bereichsleiter Valorant Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1220804206269567087)).Allow(Permissions.SendMessages), // Bereichsleiter Valorant Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                    };
                    break;

                case "ticketCS2ClanBtn":
                    ticketDesc = "Test123";
                    ticketTitle = "CS2 Clan Bewerbung";

                    overwrites = new List<DiscordOverwriteBuilder>
                    {
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(guild.GetRole(1220803957560049724)).Allow(Permissions.AccessChannels), // Bereichsleiter CS2 Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1220803957560049724)).Allow(Permissions.SendMessages), // Bereichsleiter CS2 Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                    };
                    break;
            }

            DiscordChannel channel = await guild.CreateTextChannelAsync($"{e.User.Username}-Ticket", category, overwrites: overwrites, position: 0);

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ticket erstellt: ({channel.Mention})").AsEphemeral(true));

            var closeButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeTicketButton", "🔒 Schließen");
            var closeReasonButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeReasonTicketButton", "🔒 Schließen mit Begründung");
            var claimButton = new DiscordButtonComponent(ButtonStyle.Primary, "claimTicketButton", "☑️ Beanspruchen");

            await channel.SendMessageAsync($"||{user.Mention}||");

            var ticketMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle("__" + ticketTitle + "__")
                    .WithThumbnail(guild.IconUrl)
                    .WithDescription("**In Kürze wird sich jemand um dich kümmern!**\n" +
                                     "Sollte dein Anliegen bereits erledigt sein dann drücke auf 🔒 um dein Ticket zu Schließen!\n\n" + ticketDesc)
                    )
                    .AddComponents(closeButton, closeReasonButton, claimButton);
            await channel.SendMessageAsync(ticketMessage);
        }

        public static async Task CheckIfUserHasTicketPermissions(InteractionContext ctx)
        {
            if (!CmdShortener.CheckRole(ctx, 1197912790208356422))
            {
                await CmdShortener.SendNotification(ctx, "Keine Rechte", "Du bist nicht die nötigen Rechte, um Ticketbefehle zu verweden!", DiscordColor.Red, 0);
                return;
            }
        }
        
}
}
