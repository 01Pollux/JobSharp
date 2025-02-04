using JobSharp.Core.Payload;

namespace JobSharp.Core.Services;

internal class RegisteredScrapper
{
    private readonly IJobScrapper _scrapper;
    private readonly JobSearchDescription _jobSearchDesc;
    private DateTime _lastScrapTime;

    public string Platform => _scrapper.Platform;

    public RegisteredScrapper(IJobScrapper scrapper, JobSearchDescription jobSearchDesc, DateTime lastScrapTime)
    {
        _scrapper = scrapper;
        _jobSearchDesc = jobSearchDesc;
        _lastScrapTime = lastScrapTime;
    }

    public async IAsyncEnumerable<JobDescription> GetJobs()
    {
        DateTime max = _lastScrapTime;

        await foreach (var job in _scrapper.GetJobs(_lastScrapTime, _jobSearchDesc))
        {
            if (job is not null)
            {
                if (job.PostDate > max)
                {
                    max = job.PostDate;
                }
                yield return job;
            }
        }

        if (max > _lastScrapTime)
        {
            _lastScrapTime = max.AddSeconds(1);
        }
    }
}
