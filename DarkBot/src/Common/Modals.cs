using DarkBot.src.CommandHandler;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using DSharpPlus.Interactivity;
using Microsoft.VisualBasic;

namespace DarkBot.src.Common
{
    public class Modals
    {
        public static async Task HandleModal(DiscordClient client, ModalSubmitEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.ModalSubmit
             && e.Interaction.Data.CustomId == "modalValoClanForm"
             || e.Interaction.Data.CustomId == "modalCS2ClanForm")
            {
                await Ticket_Handler.HandleGeneralTickets(e);
            }
            if (e.Interaction.Type == InteractionType.ModalSubmit
             && e.Interaction.Data.CustomId == "modalCloseReasonForm")
            {
                await Ticket_Handler.CloseTicket(e);
            }
            if (e.Interaction.Type == InteractionType.ModalSubmit
             && e.Interaction.Data.CustomId == "modalCoachingForm")
            {
                await Ticket_Handler.HandleGeneralTickets(e);
            }

        }
        
        public static async Task CreateClanModal(ComponentInteractionCreateEventArgs e, string modalId)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Clan Beitrittsformular")
                .WithCustomId(modalId)
                .AddComponents(
                    new TextInputComponent("Name / Ingamename",  "nameTextBox", value: ""))
                .AddComponents(
                    new TextInputComponent("Dein aktueller Rank", "rankTextBox", value: ""))
                .AddComponents(
                new TextInputComponent("Dein Alter", "ageTextBox", value: ""))
                .AddComponents(
                new TextInputComponent("Kurze Vorstellung von dir", "vorstellTextBox", value: ""));

            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }

        public static async Task CreateCoachingModal(ComponentInteractionCreateEventArgs e, string modalId)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Training beantragen")
                .WithCustomId(modalId)
                .AddComponents(
                    new TextInputComponent("Welche Elo bist du aktuell", "eloTextBox", value: ""))
                .AddComponents(
                    new TextInputComponent("Was soll trainiert werden", "whatTextBox", value: ""))
                .AddComponents(
                    new TextInputComponent("Nenne 3 Tage an den zu Zeit hast", "dayTextBox", value: ""));

            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }

        public static async Task CreateReasonModal(ComponentInteractionCreateEventArgs e, string modalId)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Ticket schließen")
                .WithCustomId(modalId)
                .AddComponents(
                    new TextInputComponent(label: "Grund", customId: "closeReasonTextBox", value: "")
                );

            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
