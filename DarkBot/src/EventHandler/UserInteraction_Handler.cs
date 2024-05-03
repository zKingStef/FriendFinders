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
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;

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
                    await CreateModal(e, "modalValoClanForm");
                    break;
                case "ticketCS2ClanBtn":
                    await CreateModal(e, "modalCS2ClanForm");
                    break;

                default:
                    Console.WriteLine(e.Message);
                    break;
            }
        }

        public static async Task HandleModal(DiscordClient client, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit 
             && e.Interaction.Data.CustomId == "modalValoClanForm" 
             || e.Interaction.Data.CustomId == "modalCS2ClanForm")
            {
                await Ticket_Handler.HandleGeneralTickets(e);
            }
        }

        private static async Task CreateModal(ComponentInteractionCreateEventArgs e, string modalId)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Clan Beitrittsformular")
                .WithCustomId(modalId)
                .AddComponents(
                    new TextInputComponent(label: "Kurze Vorstellung von dir...", customId: "vorstellTextBox", value: "")
                );

            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}