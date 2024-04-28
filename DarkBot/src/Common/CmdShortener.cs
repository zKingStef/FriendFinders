using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using DarkBot.src.Common;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System.IO;
using System.Net.Http;

namespace DarkBot.src.Common
{
    public static class CmdShortener
    {
        // Methode zum Senden von Benachrichtigungen
        public static async Task SendNotification(InteractionContext ctx,
                                                  string title,
                                                  string description,
                                                  DiscordColor color,
                                                  ulong channelId)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = color,
                Timestamp = DateTime.UtcNow,
            };

            if (channelId == 0)
                await ctx.CreateResponseAsync(message);
            else if (channelId == 1)
                await ctx.Channel.SendMessageAsync(message);
            else
                await ctx.Guild.GetChannel(channelId).SendMessageAsync(message);
        }

        public static async Task SendAsEphemeral(InteractionContext ctx,
                                                  string text)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                     .WithContent((text)).AsEphemeral(true));
        }

        public static async Task SendLogMessage(DiscordClient client, ulong channelId, AuditLogActionType alaType, string title, string description, DiscordColor color)
        {
            var guild = await client.GetGuildAsync(978346565209042984);
            var auditLogs = await guild.GetAuditLogsAsync(1, null, alaType);
            var lastLog = auditLogs.FirstOrDefault();
            var responsible = lastLog?.UserResponsible;

            var channel = await client.GetChannelAsync(channelId);

            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description + responsible.Mention,
                Color = color,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = responsible?.AvatarUrl,
                    Name = responsible?.Username
                }
            };

            var embed = embedBuilder.Build();
            await channel.SendMessageAsync(embed: embed);
        }

        public static async Task SendLogMessage(DiscordClient client, ulong channelId, string title, string description, DiscordColor color)
        {
            var channel = await client.GetChannelAsync(channelId);

            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = color,
            };

            var embed = embedBuilder.Build();
            await channel.SendMessageAsync(embed: embed);
        }


        public static async Task SendDirectMessage(InteractionContext ctx, DiscordMember user, string title, string description, DiscordColor color)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = title,
                Description = "**Server:** " + ctx.Guild.Name +
                              "\n**Reason:** " + description +
                              "\n\n**Responsible Moderator:** " + ctx.Member.Mention,
                Color = color,
                Timestamp = DateTime.UtcNow
            };

            var channel = await user.CreateDmChannelAsync();
            await channel.SendMessageAsync(message);
        }

        // Methode zur Berechtigungsprüfung
        public static bool CheckPermissions(InteractionContext ctx, Permissions requiredPermissions)
        {
            return ctx.Member.Permissions.HasPermission(requiredPermissions);
        }

        public static bool CheckRole(InteractionContext ctx, ulong roleId)
        {
            var member = ctx.Member;
            return member.Roles.Any(r => r.Id == roleId);
        }

        public static async Task CheckIfUserHasCeoRole(InteractionContext ctx)
        {
            if (!CmdShortener.CheckRole(ctx, 978346565225816152))
            {
                await CmdShortener.SendNotification(ctx, "No access", "You do not have the necessary permissions to execute this command.", DiscordColor.Red, 0);
                return;
            }
        }

        // Methode zur Fehlerbehandlung
        public static async Task HandleException(InteractionContext ctx, Exception e)
        {
            string errorMessage = $"Exception occured: {e.Message}";
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(errorMessage));
        }

        public static async Task SendResponseAsync(CommandContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            if (type == ResponseType.Warning)
            {
                message = ":exclamation: " + message;
            }
            else if (type == ResponseType.Missing)
            {
                message = ":mag: " + message;
            }
            else if (type == ResponseType.Error)
            {
                message = ":no_entry: " + message;
            }
            // else bleibt unverändert


            await ctx.RespondAsync(message).ConfigureAwait(false);
        }

        public static async Task SendResponseAsync(InteractionContext ctx, string message, ResponseType type = ResponseType.Default)
        {
            if (type == ResponseType.Warning)
            {
                message = ":exclamation: " + message;
            }
            else if (type == ResponseType.Missing)
            {
                message = ":mag: " + message;
            }
            else if (type == ResponseType.Error)
            {
                message = ":no_entry: " + message;
            }

            await ctx.CreateResponseAsync(message).ConfigureAwait(false);
        }

        public static async Task SendUserStateChangeAsync(CommandContext ctx, UserStateChange state, DiscordMember user,
            string reason)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription($"{state}: {user.DisplayName}#{user.Discriminator}\nIdentifier: {user.Id}\nReason: {reason}\nIssued by: {ctx.Member.DisplayName}#{ctx.Member.Discriminator}")
                .WithColor(DiscordColor.Green);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        public static bool CheckChannelName(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Length <= 100;
        }

        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(CommandContext ctx, string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }

        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(InteractionContext ctx, string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase), TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }

        public static int LimitToRange(double value, int min = 1, int max = 100)
        {
            if (value <= min) return min;
            return (int)(value >= max ? max : value);
        }

        public static async Task RemoveMessage(DiscordMessage message)
        {
            await message.DeleteAsync().ConfigureAwait(false);
        }

        public static async Task<MemoryStream> CheckImageInput(CommandContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (input != null && !Uri.TryCreate(input, UriKind.Absolute, out _) && (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
            {
                //await SendResponseAsync(ctx, Properties.Resources.URL_INVALID_IMG, ResponseType.Warning).ConfigureAwait(false);
            }
            else
            {
                HttpClient client = new();
                HttpResponseMessage? httpResponse = null;
                try
                {
                    httpResponse = await client.GetAsync(input).ConfigureAwait(false);
                }
                finally
                {
                    httpResponse?.Dispose();
                    client?.Dispose();
                }

                var result = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                stream.Write(result, 0, result.Length);
                stream.Position = 0;
            }

            return stream;
        }

        public static async Task<MemoryStream?> CheckImageInput(InteractionContext ctx, string input)
        {
            var stream = new MemoryStream();
            if (input != null && !Uri.TryCreate(input, UriKind.Absolute, out _) && (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
            {
                //await SendResponseAsync(ctx, Properties.Resources.URL_INVALID_IMG, ResponseType.Warning).ConfigureAwait(false);
            }
            else
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage httpResponse = null;
                try
                {
                    httpResponse = await client.GetAsync(input).ConfigureAwait(false);
                }
                finally
                {
                    httpResponse?.Dispose();
                    client?.Dispose();
                }

                var result = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                stream.Write(result, 0, result.Length);
                stream.Position = 0;
            }
            return (stream.Length > 0) ? stream : null;
        }

        public static string GetCurrentUptime()
        {
            var settings = Program.Settings;
            var uptime = DateTime.Now - settings.ProcessStarted;
            var days = uptime.Days > 0 ? $"({uptime.Days:00} days)" : string.Empty;
            return $"{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds} {days}";
        }
    }
}
