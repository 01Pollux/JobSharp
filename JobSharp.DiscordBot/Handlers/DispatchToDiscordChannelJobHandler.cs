using Discord;
using Discord.WebSocket;
using JobSharp.Core;
using JobSharp.Core.Payload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JobSharp.DiscordBot.Handlers;

internal class DispatchToDiscordChannelJobHandler : IJobHandler
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;

    private readonly ulong _channelId;

    public DispatchToDiscordChannelJobHandler(
        ILogger<DispatchToDiscordChannelJobHandler> logger,
        DiscordSocketClient client,
        IConfiguration config)
    {
        _logger = logger;
        _client = client;
        _channelId = config.GetValue<ulong>("Discord:JobPostChannelId");
    }

    public async Task Handle(JobDescription job)
    {
        if (_client.GetChannel(_channelId) is not ISocketMessageChannel channel)
        {
            _logger.LogError("Channel not found");
            return;
        }

        var embed = GetEmbed(job);
        await channel.SendMessageAsync(embed: embed);
    }

    private static Embed GetEmbed(JobDescription job)
    {
        return new EmbedBuilder()
            .WithTitle($"{job.Title} at {job.Company}")
            .WithUrl(job.Url)
            .WithThumbnailUrl(job.CompanyImageLink)
            .AddField("Company", $"[{job.Company}]({job.CompanyLink})", true)
            .AddField("Location", job.Location, true)
            .AddField("Not Promoted", job.IsPromoted ? ":x: No" : ":white_check_mark: Yes", true)
            .WithFooter($"Posted on {job.PostDate:yyyy-MM-dd HH:mm}")
            .WithColor(job.IsPromoted ? Color.LightOrange : Color.Green)
            .Build();
    }
}
