﻿using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace DarkBot.src.Common
{
    public class BotSettings
    {
        [JsonPropertyName("Lavalink")] public Lavalink Lavalink;
        [JsonPropertyName("Name")] public string Name;
        [JsonPropertyName("Prefix")] public string Prefix;
        [JsonPropertyName("ShardCount")] public int ShardCount;
        [JsonPropertyName("Tokens")] public Tokens Tokens;
        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public string GitHubLink { get; } //= Resources.URL_BOT_GitHub;
        public string InviteLink { get; } //= Resources.URL_BOT_Invite;
        public string DocsLink { get; } //= Resources.URL_BOT_Docs;
        public DateTime ProcessStarted { get; set; }
    }

    public class Lavalink
    {
        [JsonPropertyName("Address")] public string Address;

        [JsonPropertyName("Enabled")] public bool Enabled;

        [JsonPropertyName("Password")] public string Password;

        [JsonPropertyName("Port")] public int Port;
    }

    public class Tokens
    {
        [JsonProperty("Discord")] public string? DiscordToken { get; set; }

        [JsonProperty("Steam", NullValueHandling = NullValueHandling.Ignore)]
        public string? SteamToken { get; set; }

        [JsonProperty("Imgur", NullValueHandling = NullValueHandling.Ignore)]
        public string? ImgurToken { get; set; }

        [JsonProperty("Pokemon", NullValueHandling = NullValueHandling.Ignore)]
        public string? PokemonToken { get; set; }

        [JsonProperty("OMDB", NullValueHandling = NullValueHandling.Ignore)]
        public string? OmdbToken { get; set; }

        [JsonProperty("Twitch", NullValueHandling = NullValueHandling.Ignore)]
        public string? TwitchToken { get; set; }

        [JsonProperty("TwitchAccess", NullValueHandling = NullValueHandling.Ignore)]
        public string? TwitchAccess { get; set; }

        [JsonProperty("NASA", NullValueHandling = NullValueHandling.Ignore)]
        public string? NasaToken { get; set; }

        [JsonProperty("TeamworkTF", NullValueHandling = NullValueHandling.Ignore)]
        public string? TeamworkToken { get; set; }

        [JsonProperty("News", NullValueHandling = NullValueHandling.Ignore)]
        public string? NewsToken { get; set; }

        [JsonProperty("Weather", NullValueHandling = NullValueHandling.Ignore)]
        public string? WeatherToken { get; set; }

        [JsonProperty("YouTube", NullValueHandling = NullValueHandling.Ignore)]
        public string? YouTubeToken { get; set; }
    }

    public enum ResponseType
    {
        Default,
        Warning,
        Missing,
        Error
    }

    public enum UserStateChange
    {
        Ban,
        Deafen,
        Kick,
        Mute,
        Unban,
        Undeafen,
        Unmute
    }
}