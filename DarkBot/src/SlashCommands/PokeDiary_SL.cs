using DarkBot.src.CommandHandler;
using DarkBot.src.Common;
using DarkBot.src.Database;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DarkBot.src.SlashCommands
{
    [SlashCommandGroup("pokediary", "Slash Commands for the Pokemon Go Diary.")]
    public class PokeDiary_SL : PokeDiary
    {
        [SlashCommand("addstats", "Add daily statistics")]
        public static async Task AddStats(InteractionContext ctx,
                          [Option("date", "DateTime Format: (YYYY-MM-dd)")] string date,
                          [Option("distance", "Distance walked in kilometers")] double distance,
                          [Option("pokemon", "Number of Pokémon caught")] long pokemon,
                          [Option("pokestops", "Number of PokéStops visited")] long pokestops,
                          [Option("total_xp", "Total XP gained")] long totalXP,
                          [Option("stardust", "Amount of Stardust collected")] long stardust,
                          [Option("weekly_kilometers", "Kilometers walked in the week")] long weeklyKilometers,
                          [Option("pokecoins", "Amount of Pokecoins")] long pokecoins,
                          [Option("raidpasses", "Amount of Raidpasses")] long raidpasses,
                          [Option("shinys", "Shiny Pokemon")] long shinys,
                          [Option("legendarys", "Legendary Pokemon")] long legendarys,
                          [Option("hundos", "Hundo Pokemon")] long hundos,
                          [Option("shundos", "Shiny Hundo Pokemon")] long shundos)

        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);
            DBEngine dbEngine = new();

            try
            {
                using var conn = new NpgsqlConnection(dbEngine.connectionString);
                await conn.OpenAsync();

                string query = "INSERT INTO bmocfdpnmiqmcbuykudg.POKEDIARY " +
                               "(ENTRY_DATE, POKEMON_CAUGHT, POKESTOPS_VISITED, " +
                               "DISTANCE_WALKED, TOTAL_XP, STARDUST, " +
                               "WEEKLY_DISTANCE, POKECOINS, RAIDPASSES, " +
                               "SHINYS, LEGENDARYS, HUNDOS, SHUNDOS)" +
                               $"VALUES('{date}', {pokemon}, {pokestops}, {distance}, " +
                               $"{totalXP}, {stardust}, {weeklyKilometers}, {pokecoins}," +
                               $" {raidpasses}, {shinys}, {legendarys}, {hundos}, {shundos})";

                using var cmd = new NpgsqlCommand(query, conn);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            await ctx.CreateResponseAsync($":white_check_mark: Daily statistics for {DateTime.Now.ToShortDateString()} added successfully.");
        }


        [SlashCommand("dailystats", "Show today's statistics")]
        public async Task ShowTodayStats(InteractionContext ctx)
        {
            // Lade die täglichen Statistiken aus der JSON-Datei
            List<DailyStatsEntry> entries = LoadTodaysStats();

            // Suche nach den Statistiken für das angegebene Datum
            DailyStatsEntry stats = entries.FirstOrDefault(e => e.Date.Date == DateTime.Today.Date);

            if (stats != null)
            {
                // Wenn Statistiken für das angegebene Datum gefunden wurden, zeige sie an
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Statistics for {DateTime.Today.Date.ToShortDateString()}",
                    Color = DiscordColor.CornflowerBlue
                };

                // Hier kannst du den Embed nach deinen Wünschen formatieren und die Statistiken hinzufügen
                embed.AddField("Distance walked", $"{stats.Distance.ToString("N0")} km", true);
                embed.AddField("Pokémon caught", stats.Pokemon.ToString("N0"), true);
                embed.AddField("PokéStops visited", stats.Pokestops.ToString("N0"), true);
                embed.AddField("Total XP gained", stats.TotalXP.ToString("N0"), true);
                embed.AddField("Stardust collected", stats.Stardust.ToString("N0"), true);
                embed.AddField("Weekly kilometers", $"{stats.WeeklyKilometers} km", true);

                if (!string.IsNullOrEmpty(stats.ImageUrl))
                {
                    embed.WithImageUrl(stats.ImageUrl);
                }

                await ctx.CreateResponseAsync(embed: embed);
            }
            else
            {
                // Wenn keine Statistiken für das angegebene Datum gefunden wurden, gib eine entsprechende Nachricht aus
                await ctx.CreateResponseAsync($"No statistics found for today.");
            }
        }


        [SlashCommand("stats", "Get statistics for a specific date")]
        public async Task GetStats(InteractionContext ctx,
                                  [Option("date", "The date to get statistics for (format: yyyy-MM-dd)")] string dateString)
        {
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                // Lade die täglichen Statistiken aus der JSON-Datei
                List<DailyStatsEntry> entries = LoadSpecificStats(dateString);

                // Suche nach den Statistiken für das angegebene Datum
                DailyStatsEntry stats = entries.FirstOrDefault(e => e.Date.Date == date.Date);

                if (stats != null)
                {
                    // Wenn Statistiken für das angegebene Datum gefunden wurden, zeige sie an
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = $"Statistics for {date.ToShortDateString()}",
                        Color = DiscordColor.Magenta
                    };

                    // Hier kannst du den Embed nach deinen Wünschen formatieren und die Statistiken hinzufügen
                    embed.AddField("Distance walked", $"{stats.Distance.ToString("N0")} km", true);
                    embed.AddField("Pokémon caught", stats.Pokemon.ToString("N0"), true);
                    embed.AddField("PokéStops visited", stats.Pokestops.ToString("N0"), true);
                    embed.AddField("Total XP gained", stats.TotalXP.ToString("N0"), true);
                    embed.AddField("Stardust collected", stats.Stardust.ToString("N0"), true);
                    embed.AddField("Weekly kilometers", $"{stats.WeeklyKilometers} km", true);

                    if (!string.IsNullOrEmpty(stats.ImageUrl))
                    {
                        embed.WithImageUrl(stats.ImageUrl);
                    }

                    await ctx.CreateResponseAsync(embed: embed);
                }
                else
                {
                    // Wenn keine Statistiken für das angegebene Datum gefunden wurden, gib eine entsprechende Nachricht aus
                    await ctx.CreateResponseAsync($"No statistics found for {date.ToShortDateString()}.");
                }
            }
            else
            {
                // Wenn das angegebene Datum im falschen Format ist, gib eine Fehlermeldung aus
                await ctx.CreateResponseAsync("Invalid date format. Please use the format yyyy-MM-dd.");
            }
        }

        /*
        [SlashCommand("allstats", "Get all statistics")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public async Task ShowAllStats(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            
            // Lade die täglichen Statistiken aus der JSON-Datei
            List<DailyStatsEntry> entries = LoadTodaysStats();

            // Suche nach den Statistiken für das angegebene Datum
            DailyStatsEntry stats = entries.FirstOrDefault(e => e.Date.Date == DateTime.Today.Date);

            if (stats != null)
            {
                // Wenn Statistiken für das angegebene Datum gefunden wurden, zeige sie an
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Statistics for {DateTime.Today.Date.ToShortDateString()}",
                    Color = DiscordColor.CornflowerBlue
                };

                // Hier kannst du den Embed nach deinen Wünschen formatieren und die Statistiken hinzufügen
                embed.AddField("Distance walked", $"{stats.Distance.ToString("N0")} km", true);
                embed.AddField("Pokémon caught", stats.Pokemon.ToString("N0"), true);
                embed.AddField("PokéStops visited", stats.Pokestops.ToString("N0"), true);
                embed.AddField("Total XP gained", stats.TotalXP.ToString("N0"), true);
                embed.AddField("Stardust collected", stats.Stardust.ToString("N0"), true);
                embed.AddField("Weekly kilometers", $"{stats.WeeklyKilometers} km", true);

                if (!string.IsNullOrEmpty(stats.ImageUrl))
                {
                    embed.WithImageUrl(stats.ImageUrl);
                }

                // Erstelle die Buttons mit Emojis und benutzerdefinierten IDs
                var buttonRow = new DiscordButtonComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, "previous_day", "Previous Day", emoji: new DiscordComponentEmoji("⬅️")),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "next_day", "Next Day", emoji: new DiscordComponentEmoji("➡️"))
                };

                // Erstelle die Interaktionsantwort mit den Buttons
                var response = new DiscordInteractionResponseBuilder()
                    .AddEmbed(embed)
                    .AddComponents(buttonRow);

                var interactionResponse = await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);

                // Warte auf die Button-Interaktion
                var interactivity = ctx.Client.GetInteractivity();
                var buttonResult = await interactivity.WaitForButtonAsync(interactionResponse.Id, ctx.User);

                if (buttonResult != null)
                {
                    // Handle button interaction
                    if (buttonResult.Id == "previous_day")
                    {
                        // Logic for showing statistics for the previous day
                    }
                    else if (buttonResult.Id == "next_day")
                    {
                        // Logic for showing statistics for the next day
                    }
                }
            }
            else
            {
                // Wenn keine Statistiken für das angegebene Datum gefunden wurden, gib eine entsprechende Nachricht aus
                await ctx.CreateResponseAsync($"No statistics found for today.");
            }
        }
        */

        private static DiscordEmbedBuilder BuildStatsPageEmbed(List<DailyStatsEntry> entries, int pageIndex)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Daily Statistics",
                Color = DiscordColor.Green
            };

            int startIndex = pageIndex * 5;
            int endIndex = Math.Min(startIndex + 5, entries.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var entry = entries[i];
                embed.AddField($"{entry.Date.ToShortDateString()}",
                    $"Distance: {entry.Distance} km\n" +
                    $"Pokémon: {entry.Pokemon}\n" +
                    $"Pokestops: {entry.Pokestops}\n" +
                    $"Total XP: {entry.TotalXP}\n" +
                    $"Stardust: {entry.Stardust}\n" +
                    $"Weekly kilometers: {entry.WeeklyKilometers} km");
            }

            return embed;
        }





    }
}
