using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DarkBot.src.CommandHandler
{
    public class PokeDiary : ApplicationCommandModule
    {
        public class DailyStatsEntry
        {
            public DateTime Date { get; set; }
            public double Distance { get; set; }
            public long Pokemon { get; set; }
            public long Pokestops { get; set; }
            public long TotalXP { get; set; }
            public long Stardust { get; set; }
            public long WeeklyKilometers { get; set; }
            public string? ImageUrl { get; set; } // URL zum Bild
        }

        public void SaveDailyStats(DailyStatsEntry entry)
        {
            // Erstelle den Dateinamen mit dem heutigen Datum
            string fileName = $"daily_stats_{DateTime.Today:yyyy-MM-dd}.json";
            string directoryPath = Path.Combine("Database", "PokeDiary");
            string filePath = Path.Combine(directoryPath, fileName);

            // Speichern des Eintrags in der entsprechenden Datei
            string json = JsonConvert.SerializeObject(entry, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }


        public async Task<string> SaveImage(DiscordAttachment attachment)
        {
            // Generiere einen eindeutigen Dateinamen für das Bild
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(attachment.Url)}";
            string filePath = Path.Combine("images", fileName);

            // Erstelle das Verzeichnis, falls es noch nicht existiert
            Directory.CreateDirectory("images");

            // Lade das Bild herunter und speichere es auf dem Server
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(attachment.Url), filePath);
            }

            // Gib die URL des gespeicherten Bilds zurück
            return filePath;
        }


        public List<DailyStatsEntry> LoadTodaysStats()
        {
            // Erstelle den Dateinamen mit dem heutigen Datum
            string fileName = $"daily_stats_{DateTime.Today:yyyy-MM-dd}.json";
            string filePath = Path.Combine("Database", "PokeDiary", fileName);

            // Überprüfe, ob die Datei existiert
            if (File.Exists(filePath))
            {
                // Lade die Statistiken aus der JSON-Datei
                string json = File.ReadAllText(filePath);
                return new List<DailyStatsEntry> { JsonConvert.DeserializeObject<DailyStatsEntry>(json) };
            }
            else
            {
                // Falls die Datei nicht existiert, gibt es keine Statistiken für heute
                return new List<DailyStatsEntry>();
            }
        }


        public List<DailyStatsEntry> LoadSpecificStats(string dateString)
        {
            // Erstelle den Dateinamen mit dem heutigen Datum
            string fileName = $"daily_stats_{dateString}.json";
            string filePath = Path.Combine("Database", "PokeDiary", fileName);

            // Überprüfe, ob die Datei existiert
            if (File.Exists(filePath))
            {
                // Lade die Statistiken aus der JSON-Datei
                string json = File.ReadAllText(filePath);
                return new List<DailyStatsEntry> { JsonConvert.DeserializeObject<DailyStatsEntry>(json) };
            }
            else
            {
                // Falls die Datei nicht existiert, gibt es keine Statistiken für heute
                return new List<DailyStatsEntry>();
            }
        }

    }
}
