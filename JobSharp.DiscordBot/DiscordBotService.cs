using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobSharp.DiscordBot;

internal class DiscordBotService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _interactionHandler;
    private readonly IConfiguration _configuration;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        DiscordSocketClient client,
        InteractionHandler interactionHandler,
        IConfiguration configuration)
    {
        _logger = logger;
        _client = client;
        _interactionHandler = interactionHandler;
        _configuration = configuration;

        _client.Log += LogAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _interactionHandler.InitializeAsync();
            await _client.LoginAsync(TokenType.Bot, _configuration["Discord:AccessToken"]!);
            await _client.StartAsync();

            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the Discord bot");
        }
        finally
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }

    private Task LogAsync(LogMessage log)
    {
        switch (log.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                _logger.LogError(log.Exception, log.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(log.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation(log.Message);
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                _logger.LogDebug(log.Message);
                break;
        }

        return Task.CompletedTask;
    }
}
