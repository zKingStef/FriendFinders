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
using DarkBot.src.Common;

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
                    await Modals.CreateClanModal(e, "modalValoClanForm");
                    break;
                case "ticketCS2ClanBtn":
                    await Modals.CreateClanModal(e, "modalCS2ClanForm");
                    break;
                case "claimTicketButton":
                    if (Ticket_Handler.CheckIfUserHasTicketPermissions(e))
                    { 
                        await Ticket_Handler.RemoveClaimButtonAsync(e);
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($"Das Ticket wird jetzt von {e.User.Mention} bearbeitet"));
                    }
                    break;
                case "closeTicketButton":
                    await Ticket_Handler.CloseTicket(e);
                    break;
                case "closeReasonTicketButton":
                    await Modals.CreateReasonModal(e, "modalCloseReasonForm");
                    break;

                default:
                    Console.WriteLine(e.Message);
                    break;
            }
        }
    }
}