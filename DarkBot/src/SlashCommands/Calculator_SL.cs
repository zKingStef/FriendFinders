using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;

namespace DarkBot.src.SlashCommands
{
    public class Calculator_SL : ApplicationCommandModule
    {
        private const string ApiUrl = "https://api.exchangerate-api.com/v4/latest/TRY"; // API-URL für türkische Lira
        
        [SlashCommand("calculate", "Taschenrechner")]
        public async Task Calculate(InteractionContext ctx,
                                [Option("Number1", "Enter first number")] double num1,
                                [Option("Operator", "Operator ( + - * / )")] string op,
                                [Option("Number2", "Enter second number")] double num2)
        {
            double result = 0;

            switch (op)
            {
                case "+":
                    result = num1 + num2;
                    break;

                case "-":
                    result = num1 - num2;
                    break;

                case "*":
                    result = num1 * num2;
                    break;

                case "/":
                    result = num1 / num2;
                    break;

                default:
                    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                     .WithContent(($"Das ist kein Rechenzeichen! Benutze ( +, -, *, / )")).AsEphemeral(true));
                    break;
            }

            var calculatorEmbed = new DiscordEmbedBuilder
            {
                Title = "**Taschenrechner**",
                Color = DiscordColor.CornflowerBlue,
                Description = $"{num1} {op} {num2} = **{result}**",
                Timestamp = DateTime.UtcNow,
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(calculatorEmbed.Build()));
        }
    }
}
