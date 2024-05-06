using DarkBot.src.Common;
using DarkBot.src.Handler;
using DarkBot.src.Logs;

//using DarkBot.src.Logs;
using DarkBot.src.PrefixCommands;
using DarkBot.src.SlashCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DarkBot
{
    internal sealed class DarkBot
    {
        private static DiscordClient? Client { get; set; }
        private IServiceProvider Services { get; }
        private static EventId EventId { get; } = new (1000, Program.Settings.Name);
        private CommandsNextExtension Commands { get; }
        private InteractivityExtension Interactivity { get; }
        private VoiceNextExtension Voice { get; }
        private LavalinkExtension Lavalink { get; }
        private SlashCommandsExtension Slash { get; }

        public DarkBot(int shardId = 0)
        {
            // Get Settings
            var settings = Program.Settings;

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = settings.Tokens.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                MinimumLogLevel = LogLevel.Information,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                LargeThreshold = 250,
                MessageCacheSize = 2048,
                LogTimestampFormat = "yyyy-MM-dd HH:mm:ss zzz",
                ShardId = shardId,
                ShardCount = settings.ShardCount
            });


            // Setup Services
            Services = new ServiceCollection()
                //.AddSingleton<MusicService>()
                //.AddSingleton(new LavalinkService(Client))
                .AddSingleton(this)
                .BuildServiceProvider(true);

            // Setup Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                Services = Services,
                PrefixResolver = PrefixResolverAsync, // Set the command prefix that will be used by the bot
                EnableMentionPrefix = true,
                EnableDms = false,
                EnableDefaultHelp = true,
            });
            //Commands.CommandExecuted += Command_Executed;
            //Commands.CommandErrored += Command_Errored;
            //Commands.SetHelpFormatter<HelpFormatter>();

            //Commands.RegisterCommands<Misc_PX>();

            //4. Set the default timeout for Commands that use interactivity
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromSeconds(30)
            });

            Voice = Client.UseVoiceNext(new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default,
                EnableIncoming = true
            });

            Slash = Client.UseSlashCommands();
            Client.GetSlashCommands();
            //Slash.RegisterCommands<AutoRole_SL>();
            Slash.RegisterCommands<Calculator_SL>();
            //Slash.RegisterCommands<DarkServices_SL>();
            //Slash.RegisterCommands<ImgFinder_SL>();
            Slash.RegisterCommands<MiniGame_SL>();
            Slash.RegisterCommands<Misc_SL>();
            //Slash.RegisterCommands<Moderation_SL>();
            //Slash.RegisterCommands<PokeDiary_SL>();
            Slash.RegisterCommands<Ticket_SL>();
            //Slash.RegisterCommands<Troll_SL>();
            Slash.SlashCommandErrored += SlashCommandErrored;

            Client.ComponentInteractionCreated += UserInteraction_Handler.HandleInteraction;
            Client.ModalSubmitted += Modals.HandleModal;

            Client.GuildMemberAdded += JoinLeaveLogs.UserJoin;
            Client.GuildMemberRemoved += JoinLeaveLogs.UserLeave;
            
            Client.UnknownEvent += UnknownEvent;
            Client.ClientErrored += ClientErrored;

            Client.InviteCreated += InviteLogs.InviteCreated;
            Client.InviteDeleted += InviteLogs.InviteDeleted;

            Client.GuildBanAdded += UnBanLogs.UserBanned;
            Client.GuildBanRemoved += UnBanLogs.UserUnbanned;

            // Start the uptime counter
            Console.Title = $"{settings.Name}-{settings.Version}";
            settings.ProcessStarted = DateTime.Now;

            Task.Delay(-1);
        }

        private Task ClientErrored(DiscordClient sender, ClientErrorEventArgs e)
        {
            Client.Logger.LogError(EventId, "Bot errored...");
            Client.Logger.LogError(EventId, e.EventName);
            return Task.CompletedTask;
        }

        private Task UnknownEvent(DiscordClient sender, DSharpPlus.EventArgs.UnknownEventArgs e)
        {
            Client.Logger.LogError(EventId, "UnkownEvent occured...");
            Client.Logger.LogError(EventId, e.EventName);
            return Task.CompletedTask;
        }

        public Task SlashCommandErrored(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e)
        {
            Client.Logger.LogError(EventId, "SlashCommand did not execute...");
            Client.Logger.LogError(EventId, $"Exception: {e.Exception}");
            return Task.CompletedTask;
        }

        public static async Task RunAsync()
        {
            // Set the initial activity and connect the bot to Discord
            var act = new DiscordActivity("FriendFinders", ActivityType.Playing);
            await Client.ConnectAsync(act, UserStatus.Online).ConfigureAwait(false);
        }

        public static async Task StopAsync()
        {
            await Client.DisconnectAsync().ConfigureAwait(false);
        }

        private Task<int> PrefixResolverAsync(DiscordMessage m)
        {
            return Task.FromResult(m.GetStringPrefixLength(Program.Settings.Prefix));
        }
    }
}
