using JobSharp.Core.Payload;

namespace JobSharp.Core;

public interface IJobScrapper
{
    string Platform { get; }
    IAsyncEnumerable<JobDescription> GetJobs(DateTime lastScrapTime, JobSearchDescription jobSearch);
}
