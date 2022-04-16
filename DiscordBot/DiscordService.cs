using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules;

namespace DiscordBot
{
    public class DiscordService : BackgroundService
    {
        private DiscordSocketClient _client;
        private InteractionService _interactionService;
        private ILogger<DiscordService> _logger;
        private ISecretManager _secretManager;

        public DiscordService(ILogger<DiscordService> logger, ISecretManager secretManager)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            });
            _logger = logger;
            _secretManager = secretManager;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string secret = _secretManager.GetDiscordBotSecret();
            _logger.LogInformation($"DO NOT DO THIS: Token is \"{secret}\"");
            _client.Log += _client_Log;
            _client.Ready += _client_Ready;
            await _client.LoginAsync(TokenType.Bot, secret);
            await _client.StartAsync();
        }

        private async Task _client_Ready()
        {
            _interactionService = new InteractionService(_client);

            await _interactionService.AddModuleAsync<EchoModule>(null);
            await _interactionService.AddModuleAsync<TutorModule>(null);
            await _interactionService.RegisterCommandsToGuildAsync(_secretManager.GetSecret<ulong>("DISCORD_BOT_GUILDID"));

            _client.InteractionCreated += async interaction =>
            {
                var ctx = new SocketInteractionContext(_client, interaction);
                await _interactionService.ExecuteCommandAsync(ctx, null);
            };
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
            await _client.DisposeAsync();
        }

        private Task _client_Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Error:
                    _logger.LogError(msg.ToString());
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(msg.ToString());
                    break;
                case LogSeverity.Critical:
                    _logger.LogCritical(msg.ToString());
                    break;
                default:
                    _logger.LogInformation(msg.ToString());
                    break;
            }
            return Task.CompletedTask;
        }
    }
}