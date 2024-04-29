using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkBot.src.CommandHandler;

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
                    Ticket_Handler.HandleGeneralTickets(e);
                    break;

                case "ticketCS2ClanBtn":
                    Ticket_Handler.HandleGeneralTickets(e);
                    break;

                default:
                    break;
            }
        }
    }
}
