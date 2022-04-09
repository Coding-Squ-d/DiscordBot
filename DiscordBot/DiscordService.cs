using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class DiscordService : BackgroundService
    {
        private DiscordSocketClient _client;
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
            string secret = Environment.GetEnvironmentVariable("DISCORD_BOT_SECRET", EnvironmentVariableTarget.User | EnvironmentVariableTarget.Process);
            _client.Log += _client_Log;
            _client.Ready += _client_Ready;
            await _client.LoginAsync(TokenType.Bot, secret);
            await _client.StartAsync();
        }

        private Task _client_Ready()
        {
            return Task.CompletedTask;
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