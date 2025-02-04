using JobSharp.Core.Payload;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace JobSharp.Core.Services;

internal static class RegisteredScrapperRunner
{
    public static async Task<bool> Run(
       IReadOnlyList<RegisteredScrapper> scrappers,
       ConcurrentBag<JobDescription> jobResults,
       CancellationToken ct)
    {
        jobResults.Clear();

        foreach (var scrapper in scrappers)
        {
            if (ct.IsCancellationRequested)
            {
                return false;
            }
            await ProcessJobSearchAsync(scrapper, jobResults, ct);
        }
        
        return !jobResults.IsEmpty;
    }

    private static async Task ProcessJobSearchAsync(
        RegisteredScrapper scrapper,
        ConcurrentBag<JobDescription> jobResults,
        CancellationToken ct)
    {
        await foreach (var job in scrapper.GetJobs().WithCancellation(ct))
        {
            jobResults.Add(job);
        }
    }
}
