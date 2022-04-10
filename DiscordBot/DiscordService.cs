using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot
{
    public class DiscordService : BackgroundService
    {
        private DiscordSocketClient _client;
        private InteractionService _interactionService;
        private ILogger<DiscordService> _logger;

        public DiscordService(ILogger<DiscordService> logger)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Debug
            });
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string secret;
            if(Environment.GetEnvironmentVariable("DISCORD_BOT_WINDOWS_SCOPE") == "User")
            {
                secret = Environment.GetEnvironmentVariable("DISCORD_BOT_SECRET", EnvironmentVariableTarget.User);
            }
            else if (Environment.GetEnvironmentVariable("DISCORD_BOT_WINDOWS_SCOPE") == "Machine")
            {
                secret = Environment.GetEnvironmentVariable("DISCORD_BOT_SECRET", EnvironmentVariableTarget.Machine);
            }
            else
            {
                secret = Environment.GetEnvironmentVariable("DISCORD_BOT_SECRET");
            }
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
            await _interactionService.RegisterCommandsToGuildAsync(957341221733404782);
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
            _client.Dispose();
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