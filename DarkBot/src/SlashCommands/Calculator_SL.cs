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
        
        [SlashCommand("calculate", "Calculator")]
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
                                                     .WithContent(($"That is not a mathimatical operator. Please use ( +, -, *, / )")).AsEphemeral(true));
                    break;
            }

            var calculatorEmbed = new DiscordEmbedBuilder
            {
                Title = "**Calculator**",
                Color = DiscordColor.CornflowerBlue,
                Description = $"{num1} {op} {num2} = **{result}**",
                Timestamp = DateTime.UtcNow,
            };

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(calculatorEmbed.Build()));
        }

        [SlashCommand("try-to-eur", "Converts Turkish Lira (TRY) to Euro (EUR).")]
        public async Task ConvertTryToEur(InteractionContext ctx, [Option("amount", "The amount of Turkish Lira to convert")] double amount)
        {
            try
            {
                // HTTP-Anforderung an die API senden
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // JSON-Daten lesen und EUR zu TRY Wechselkurs extrahieren
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic? data = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                    double exchangeRate = data.rates.EUR;

                    // Umrechnung durchführen und auf 2 Nachkommastellen runden
                    double result = Math.Round(amount * exchangeRate, 2);

                    // Nachricht mit dem Ergebnis senden
                    var calculatorEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Turkish Lira Converter ",
                        Color = DiscordColor.CornflowerBlue,
                        Description = $"**{amount}** TRY is currently equal to **{result}**€."
                    };

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(calculatorEmbed.Build()));
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent($""));
                }
                else
                {
                    // Fehlermeldung senden, wenn die API-Anfrage fehlschlägt
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Failed to retrieve exchange rate data."));
                }
            }
            catch (Exception ex)
            {
                // Fehlermeldung senden, wenn ein Fehler auftritt
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"An error occurred: {ex.Message}"));
            }
        }
    }
}
