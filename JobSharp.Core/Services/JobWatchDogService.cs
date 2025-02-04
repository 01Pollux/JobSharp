using JobSharp.Core.Payload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace JobSharp.Core.Services;

public sealed class JobWatchDogService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IReadOnlyList<IJobHandler> _handlers;
    private readonly IReadOnlyList<RegisteredScrapper> _scrappers;

    private readonly TimeSpan _refreshTime;
    private readonly ConcurrentBag<JobDescription> _jobs = [];

    public JobWatchDogService(
        ILogger<JobWatchDogService> logger,
        IEnumerable<IJobHandler> handlers,
        IEnumerable<IJobScrapper> scrappers,
        IEnumerable<JobSearchDescription> jobSearchDescriptions,
        IConfiguration configuration)
    {
        _logger = logger;
        _refreshTime = TimeSpan.FromMinutes(configuration.GetValue("RefreshTimeInMin", 10));

        DateTime initialScrapTime = DateTime.Now - _refreshTime;
        _handlers = handlers.ToList();
        _scrappers = jobSearchDescriptions
            .SelectMany(jsd => scrappers.Select(s => new RegisteredScrapper(s, jsd, initialScrapTime)))
            .ToList();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {

            using var scope = _logger.BeginScope("Job scraping cycle");
            try
            {
                await FetchJobs(ct);
                if (_jobs.Count > 0)
                {
                    await ProcessJobs();
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in job watchdog cycle");
            }
            await Task.Delay(_refreshTime, ct);
        }
    }

    private async Task FetchJobs(CancellationToken ct)
    {
        await RegisteredScrapperRunner.Run(_scrappers, _jobs, ct);
    }

    private Task ProcessJobs()
    {
        return RegisteredHandlerRunner.Run(_handlers, _jobs, _logger);
    }
}