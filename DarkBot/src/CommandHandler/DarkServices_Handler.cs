using DarkBot.src.Common;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.CommandHandler
{
    public class DarkServices_Handler
    {
        private const string JsonFilePath = "Database\\DarkServices\\darkservices_data.json";

        public class ServiceEntry
        {
            public string Service { get; set; }
            public decimal Price { get; set; }
            public decimal LyraPrice { get; set; }
            public decimal SellPrice { get; set; }
            public decimal Profit { get; set; }
        }

        public static List<ServiceEntry> LoadTableFromJson()
        {
            if (File.Exists(JsonFilePath))
            {
                var json = File.ReadAllText(JsonFilePath);
                return JsonConvert.DeserializeObject<List<ServiceEntry>>(json);
            }
            else
            {
                return []; // new List<ServiceEntry>();
            }
        }

        public static void SaveTableToJson(List<ServiceEntry> table)
        {
            var json = JsonConvert.SerializeObject(table, Formatting.Indented);
            File.WriteAllText(JsonFilePath, json);
        }

        public static DiscordEmbed BuildTableEmbed(List<ServiceEntry> table)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Pricelist Table",
                Color = DiscordColor.Green
            };

            var sb = new StringBuilder();

            sb.AppendLine("```md");
            sb.AppendLine("# Service               | Costs   | Lyra   | Sell   | Profit ");
            sb.AppendLine("-------------------------------------------------------------");

            foreach (var entry in table)
            {
                // Formatierung der Preise und des Profits mit zwei Nachkommastellen
                string priceFormatted = $"{entry.Price:F2}".PadRight(7);
                string lyraPriceFormatted = $"{entry.LyraPrice:F2}".PadRight(6);
                string darkPriceFormatted = $"{entry.SellPrice:F2}".PadRight(6);
                string profitFormatted = $"{entry.Profit:F2}";

                sb.AppendLine($"{entry.Service.PadRight(23)} | {priceFormatted} | {lyraPriceFormatted} | {darkPriceFormatted} | {profitFormatted}  ");
            }

            sb.AppendLine("```");

            embed.Description = sb.ToString();

            return embed.Build();
        }
    }
}
