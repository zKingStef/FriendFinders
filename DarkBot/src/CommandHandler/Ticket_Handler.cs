using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DarkBot.src.Common;
using DSharpPlus.SlashCommands;
using System.ComponentModel.Design;
using System.Text;

namespace DarkBot.src.CommandHandler
{
    public class Ticket_Handler
    {
        private static DiscordMessage ticketMessage;

        private static Dictionary<ulong, ulong> ticketChannelMap = new Dictionary<ulong, ulong>();

        public string username { get; set; }
        public string  issue { get; set; }
        public ulong ticketId { get; set; }
        public static async Task HandleGeneralTickets(ModalSubmitEventArgs e)
        {
            DiscordMember? user = e.Interaction.User as DiscordMember;
            DiscordGuild guild = e.Interaction.Guild;

            if (guild.GetChannel(1197912790208356422) is not DiscordChannel category || category.Type != ChannelType.Category)
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Fehler beim Erstellen des Tickets: Eine Kategorie für Tickets konnte nicht gefunden werden.").AsEphemeral(true));
                return;
            }

            string ticketDesc = "Fehler! Bite melde das bei einem Techniker";
            string ticketTitle = "Fehler!";
            ulong roleId = 999999999999;
            var closeButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeTicketButton", "🔒 Schließen");
            var closeReasonButton = new DiscordButtonComponent(ButtonStyle.Secondary, "closeReasonTicketButton", "🔒 Schließen mit Begründung");
            var claimButton = new DiscordButtonComponent(ButtonStyle.Primary, "claimTicketButton", "☑️ Beanspruchen");
            DiscordChannel ticketChannel = e.Interaction.Channel;

            var overwrites = new List<DiscordOverwriteBuilder>
            {
                new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
            };

            switch (e.Interaction.Data.CustomId)
            {
                case "modalValoClanForm":
                    ticketDesc = $"**Name / Ingamename:** {e.Values["nameTextBox"]}\n" +
                                 $"**Alter:** {e.Values["ageTextBox"]}\n" +
                                 $"**Aktueller Rank:** {e.Values["rankTextBox"]}\n" +
                                 $"**Kurze Vorstellung:** {e.Values["vorstellTextBox"]}\n\n" +
                                 "Der __Bereichsleiter__ wird sich sobald wie möglich um deine Bewerbung kümmern!";
                    ticketTitle = "Valorant Clan Bewerbung";

                    roleId = 1220804206269567087;

                    overwrites =
                    [
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                        new DiscordOverwriteBuilder(guild.GetRole(roleId)).Allow(Permissions.AccessChannels), // Bereichsleiter Valorant Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                    ];
                    break;

                case "modalCS2ClanForm":
                    ticketDesc = $"**Name / Ingamename:** {e.Values["nameTextBox"]}\n" +
                                 $"**Alter:** {e.Values["ageTextBox"]}\n" +
                                 $"**Aktueller Rank:** {e.Values["rankTextBox"]}\n" +
                                 $"**Kurze Vorstellung:** {e.Values["vorstellTextBox"]}\n\n" +
                                 "Der __Bereichsleiter__ wird sich sobald wie möglich um deine Bewerbung kümmern!";
                    ticketTitle = "CS2 Clan Bewerbung";

                    roleId = 1220803957560049724;

                    overwrites =
                    [
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(guild.GetRole(1220803957560049724)).Allow(Permissions.AccessChannels), // Bereichsleiter CS2 Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                    ];
                    break;

                case "modalCoachingForm":
                    overwrites =
                    [
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(guild.GetRole(1207357073025794079)).Allow(Permissions.AccessChannels), // Coach Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                    ];

                    // Die ID der Kategorie, in der der Sprachkanal erstellt werden soll
                    ulong categoryId = 1245048697822249090;

                    // Holen Sie sich die Kategorie anhand der ID
                    DiscordChannel voiceCategory = guild.GetChannel(categoryId);

                    // Erstellen Sie den Sprachkanal innerhalb der angegebenen Kategorie
                    DiscordChannel coachingVoice = await guild.CreateVoiceChannelAsync(
                        $"Coaching-{e.Interaction.User.Username}",
                        voiceCategory,
                        overwrites: overwrites,
                        position: 0
                    );

                    ticketDesc = $"**Aktuelle Elo:** {e.Values["eloTextBox"]}\n" +
                                                     $"**Ich möchte folgendes trainieren:** {e.Values["whatTextBox"]}\n" +
                                                     $"**An diesen Tagen habe ich Zeit:** {e.Values["dayTextBox"]}\n\n" +
                                                     "Ein __Coach__ wird sich sobald wie möglich bei dir melden!\n\n" +
                                                     $"Dein persönlicher Coaching Sprachkanal: <#{coachingVoice.Id}>";
                    ticketTitle = "Valorant Coaching";

                    roleId = 1207357073025794079;

                    ticketChannel = await guild.CreateTextChannelAsync($"{e.Interaction.User.Username}-Ticket", category, overwrites: overwrites, position: 0);

                    ticketChannelMap[ticketChannel.Id] = coachingVoice.Id;
                    break;
                case "modalTechnicForm":
                    ticketDesc = $"**Problem:** {e.Values["issueTextBox"]}\n\n" +
                                 "Danke für deine Anfrage. Wir werden uns sobald wie möglich bei dir melden!";
                    ticketTitle = "Technische Hilfe";

                    roleId = 1209284430229803008;

                    overwrites =
                    [
                        new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder(guild.GetRole(1183217936513630229)).Allow(Permissions.AccessChannels), // Gründer Rolle
                        new DiscordOverwriteBuilder(guild.GetRole(1209284430229803008)).Allow(Permissions.AccessChannels), // Techniker Rolle
                        new DiscordOverwriteBuilder(user).Allow(Permissions.AccessChannels).Deny(Permissions.None),
                    ];
                    break;
            }

            if (e.Interaction.Data.CustomId != "modalCoachingForm")
            {
                ticketChannel = await guild.CreateTextChannelAsync($"{e.Interaction.User.Username}-Ticket", category, overwrites: overwrites, position: 0);
            }
         
            //var random = new Random();
            //
            //ulong minValue = 1000000000000000000;
            //ulong maxValue = 9999999999999999999;
            //
            //ulong randomNumber = (ulong)random.Next((int)(minValue >> 32), int.MaxValue) << 32 | (ulong) random.Next(); 
            //ulong result = randomNumber % (maxValue - minValue + 1) + minValue;

            //var supportTicket = new Ticket_Handler()
            //{
            //    username = e.Interaction.User.Username,
            //    issue = e.Values.Values.First(),
            //    ticketId = 1 //result
            //};

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ticket erstellt: {ticketChannel.Mention}").AsEphemeral(true));

            var ticketEmbed = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Cyan)
                    .WithTitle($"__{ticketTitle}__")
                    .WithThumbnail(guild.IconUrl)
                    .WithDescription(ticketDesc))
                    .AddComponents(closeButton, closeReasonButton, claimButton);

            // Mention the User in the Chat and then remove the Message
            var mentionMessage = await ticketChannel.SendMessageAsync(user.Mention + $"<@&{roleId}>");
            await ticketChannel.DeleteMessageAsync(mentionMessage);

            await ticketChannel.SendMessageAsync(ticketEmbed);
        }

        public static async Task RemoveClaimButtonAsync(ComponentInteractionCreateEventArgs e)
        {
            // Überprüfen, ob der Button claimTicketButton angeklickt wurde
            if (e.Interaction.Data.CustomId == "claimTicketButton")
            {
                // Überprüfen, ob die ticketMessage vorhanden ist und einen claimButton enthält
                if (ticketMessage != null && ticketMessage.Components.Any(c => c.CustomId == "claimTicketButton"))
                {
                    // Entfernen des claimButton aus der Nachricht
                    var components = ticketMessage.Components.ToList();
                    var claimButtonIndex = components.FindIndex(c => c.CustomId == "claimTicketButton");
                    components.RemoveAt(claimButtonIndex);

                    // Bearbeiten der Nachricht, um den entfernten Button anzuwenden
                    await ticketMessage.ModifyAsync(message =>
                    {
                        message.ClearComponents();
                        foreach (var component in components)
                        {
                            message.AddComponents(component);
                        }
                    });
                }
            }
        }



        public static async Task CloseTicket(ComponentInteractionCreateEventArgs e)
        {
            if (!CheckIfUserHasTicketPermissions(e))
                return;

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "🔒 Ticket geschlossen!",
                Description = $"Das Ticket wurde von {e.User.Mention} geschlossen!\n" +
                              $"Der Kanal wird in <t:{DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds()}:R> gelöscht.",
                Timestamp = DateTime.UtcNow
            };

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                     new DiscordInteractionResponseBuilder().AddEmbed(embedMessage));


            var messages = await e.Channel.GetMessagesAsync(999);

            var content = new StringBuilder();
            content.AppendLine($"Transcript Ticket {e.Channel.Name}:");
            foreach (var message in messages)
            {
                content.AppendLine($"{message.Author.Username} ({message.Author.Id}) - {message.Content}");
            }

            await Task.Delay(TimeSpan.FromSeconds(60));

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                var msg = await new DiscordMessageBuilder()
                    .AddFile($"{e.Interaction.Channel.Name}.txt", memoryStream)
                    .SendAsync(e.Guild.GetChannel(1209297588915015730));
            }



            var ticketChannelId = e.Channel.Id;
            var guild = e.Guild;
            var ticketChannel = guild.GetChannel(ticketChannelId);
            await ticketChannel.DeleteAsync("Ticket geschlossen");

            if (ticketChannelMap.TryGetValue(ticketChannelId, out var voiceChannelId))
            {
                var voiceChannel = guild.GetChannel(voiceChannelId);
                await voiceChannel.DeleteAsync("Ticket geschlossen");

                // Remove the entry from the dictionary
                ticketChannelMap.Remove(ticketChannelId);
            }

            //await e.Channel.DeleteAsync("Ticket geschlossen");
        }

        public static async Task CloseTicket(ModalSubmitEventArgs e)
        {
            if (!Ticket_Handler.CheckIfUserHasTicketPermissions(e))
                return;

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = "🔒 Ticket geschlossen!",
                Description = $"Das Ticket wurde von {e.Interaction.User.Mention} mit dem Grund **{e.Values.Values.First()}** geschlossen!\n" +
                              $"Der Kanal wird in <t:{DateTimeOffset.UtcNow.AddSeconds(60).ToUnixTimeSeconds()}:R> gelöscht.",
                Timestamp = DateTime.UtcNow
            };

            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                     new DiscordInteractionResponseBuilder().AddEmbed(embedMessage));


            var messages = await e.Interaction.Channel.GetMessagesAsync(999);

            var content = new StringBuilder();
            content.AppendLine($"Ticket geschlossen von {e.Interaction.User.Mention} mit dem Grund {e.Values.Values.First()}\n" +
                               $"Transcript Ticket {e.Interaction.Channel.Name}:");
            foreach (var message in messages)
            {
                content.AppendLine($"{message.Author.Username} ({message.Author.Id}) - {message.Content}");
            }

            await Task.Delay(TimeSpan.FromSeconds(60));

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString())))
            {
                var msg = await new DiscordMessageBuilder()
                    .AddFile($"{e.Interaction.Channel.Name}.txt", memoryStream)
                    .SendAsync(e.Interaction.Guild.GetChannel(1209297588915015730));
            }

            await e.Interaction.Channel.DeleteAsync(e.Values.Values.First());
        }

        public static async Task CheckIfUserHasTicketPermissions(InteractionContext ctx) 
        {
            if (!(CmdShortener.CheckRole(ctx, 1183217936513630229) // Gründer Rolle
             || !CmdShortener.CheckRole(ctx, 1209284430229803008) // Techniker Rolle
             || !CmdShortener.CheckRole(ctx, 1220803957560049724) // CS2 Bereichsleiter
             || !CmdShortener.CheckRole(ctx, 1220804206269567087))) // Valo Bereichsleiter 
            {
                await CmdShortener.SendNotification(ctx, "Keine Rechte", "Du hast nicht die nötigen Rechte, um Ticketbefehle zu verweden!", DiscordColor.Red, 0);
                return;
            }
        }
        
        public static bool CheckIfUserHasTicketPermissions(ComponentInteractionCreateEventArgs ctx) 
        {
            if (!(CmdShortener.CheckRole(ctx, 1183217936513630229) // Gründer Rolle
               || CmdShortener.CheckRole(ctx, 1209284430229803008) // Techniker Rolle
               || CmdShortener.CheckRole(ctx, 1220803957560049724) // CS2 Bereichsleiter
               || CmdShortener.CheckRole(ctx, 1220804206269567087))) // Valo Bereichsleiter 
            {
                CmdShortener.SendAsEphemeral(ctx, "Du hast keine Rechte dafür!");
                return false;
            }
            return true;

        }

        public static bool CheckIfUserHasTicketPermissions(ModalSubmitEventArgs ctx)
        {
            if (!(CmdShortener.CheckRole(ctx, 1183217936513630229) // Gründer Rolle
             ||  CmdShortener.CheckRole(ctx, 1209284430229803008) // Techniker Rolle
             ||  CmdShortener.CheckRole(ctx, 1220803957560049724) // CS2 Bereichsleiter
             ||  CmdShortener.CheckRole(ctx, 1220804206269567087))) // Valo Bereichsleiter 
            {
                CmdShortener.SendAsEphemeral(ctx, "Du hast keine Rechte dafür!");
                return false;
            }
            return true;
        }
    }
}
