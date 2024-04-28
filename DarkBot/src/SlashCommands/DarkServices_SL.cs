using DarkBot.src.Common;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DarkBot.src.CommandHandler.DarkServices_Handler;

namespace DarkBot.src.SlashCommands
{
    [SlashCommandGroup("darkservices", "Slash Commands Dark Services")]
    public class DarkServices_SL : ApplicationCommandModule
    {
        [SlashCommand("table", "Show Dark Services Table.")]
        public static async Task CurrencyTable(InteractionContext ctx)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            var table = LoadTableFromJson();
            var embed = BuildTableEmbed(table);

            await ctx.CreateResponseAsync(embed: embed);
        }

        [SlashCommand("addservice", "Add a new service to the table")]
        public static async Task AddService(InteractionContext ctx, [Option("service", "The name of the service to add")] string service)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            var table = LoadTableFromJson();

            // Add the new service to the table
            table.Add(new ServiceEntry { Service = service, Price = 0, SellPrice = 0, Profit = 0 });

            SaveTableToJson(table);

            await ctx.CreateResponseAsync($"New entry added: {service}");
        }

        [SlashCommand("addentry", "Add a new entry to the table")]
        public static async Task AddEntry(InteractionContext ctx,
                                         [Option("service", "The name of the service")] string service,
                                         [Option("price", "The price in Euro")] double price,
                                         [Option("lyraprice", "The price in Turkish Lyra")] double lyraPrice,
                                         [Option("sellprice", "The sell price in Euro")] double sellPrice)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            // Calculate profit
            double profit = Math.Round(sellPrice - lyraPrice, 2);

            // Create new entry
            var entry = new ServiceEntry
            {
                Service = service,
                Price = (decimal)price,
                LyraPrice = (decimal)lyraPrice,
                SellPrice = (decimal)sellPrice,
                Profit = (decimal)profit
            };

            // Load existing table
            var table = LoadTableFromJson();

            // Add new entry to table
            table.Add(entry);

            // Save updated table to JSON
            SaveTableToJson(table);

            // Respond with success message
            await ctx.CreateResponseAsync($"New entry added: {service} (Price: {price}€, LyraPrice: {lyraPrice}€, DarkPrice: {sellPrice}€, Profit: {profit}€)");
        }

        [SlashCommand("removeentry", "Remove a service from the table")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public static async Task RemoveService(InteractionContext ctx, [Option("service", "The name of the service to remove")] string service)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            var table = LoadTableFromJson();

            // Find the entry with the specified service name and remove it from the table
            var entryToRemove = table.FirstOrDefault(e => e.Service.Equals(service, StringComparison.OrdinalIgnoreCase));
            if (entryToRemove != null)
            {
                table.Remove(entryToRemove);
                SaveTableToJson(table);

                await ctx.CreateResponseAsync($"Service \"{service}\" removed from the table.");
            }
            else
            {
                await ctx.CreateResponseAsync($"Service \"{service}\" not found in the table.");
            }
        }

        [SlashCommand("modifyentry", "Modify an existing entry in the table")]
        [RequireRoles(RoleCheckMode.Any, "🧰 CEO")]
        public static async Task ModifyEntry(InteractionContext ctx,
                                            [Option("service", "The name of the service to modify")] string service,
                                            [Option("price", "The new price in Euro")] double? price = null,
                                            [Option("lyraprice", "The new Lyra price in Euro")] double? lyraPrice = null,
                                            [Option("sellprice", "The new sell price in Euro")] double? sellPrice = null)
        {
            await CmdShortener.CheckIfUserHasCeoRole(ctx);

            var table = LoadTableFromJson();

            var entry = table.FirstOrDefault(e => e.Service.Equals(service, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                await ctx.CreateResponseAsync($"Service \"{service}\" not found in the table.");
                return;
            }

            // Update entry fields
            if (price.HasValue)
                entry.Price = (decimal)price.Value;
            if (lyraPrice.HasValue)
                entry.LyraPrice = (decimal)lyraPrice.Value;
            if (sellPrice.HasValue)
                entry.SellPrice = (decimal)sellPrice.Value;

            // Recalculate profit
            entry.Profit = Math.Round(entry.SellPrice - entry.LyraPrice, 2);

            // Save updated table to JSON
            SaveTableToJson(table);

            // Respond with success message
            await ctx.CreateResponseAsync($"Entry \"{service}\" modified successfully.");
        }

    }
}
