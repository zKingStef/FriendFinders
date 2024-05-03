using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkBot.src.CommandHandler;
using DSharpPlus.Interactivity.Extensions;

namespace DarkBot.src.Handler
{
    public static class UserInteraction_Handler
    {
        public static async Task HandleInteraction(DiscordClient client, ComponentInteractionCreateEventArgs e)
        {
            var selectedOption = e.Interaction.Data.Values.FirstOrDefault();

            switch (selectedOption)
            {
                case "Template1":
                    break;

                default:
                    break;
            }

            switch (e.Interaction.Data.CustomId)
            {
                case "ticketValoClanBtn":
                case "ticketCS2ClanBtn":
                    await CreateModal(e);
                    await Ticket_Handler.HandleGeneralTickets(e);
                    break;

                default:
                    Console.WriteLine(e.Message);
                    break;
            }
        }

        public static async Task HandleModal(DiscordClient client, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit)
            {
                var values = e.Values;
                await e.Interaction.CreateResponseAsync(    InteractionResponseType.ChannelMessageWithSource, 
                                                            new DiscordInteractionResponseBuilder().WithContent(
                                                            $"{e.Interaction.User.Username} hat ein Ticket eröffnet!\n{values.First()}"));
            }
        }

        private static async Task CreateModal(ComponentInteractionCreateEventArgs e)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Beitrittsformular")
                .WithCustomId("modalClan")
                .AddComponents(
                    new TextInputComponent(label: "Kurze Vorstellung von dir", customId: "vorstellTextBox", value: "")
                );

            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}