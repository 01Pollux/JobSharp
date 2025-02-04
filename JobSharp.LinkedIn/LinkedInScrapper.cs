using JobSharp.Core;
using JobSharp.Core.Payload;
using JobSharp.LinkedIn.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace JobSharp.LinkedIn;

public partial class LinkedInScrapper : IJobScrapper, IDisposable
{
    private readonly IConfiguration _configuration;

    private readonly int _maxFetchRetry = 2;
    private readonly int _maxPagesToScrape = 10;
    private readonly int _maxJobsToFetch = 25;
    private readonly bool _runHeadless = true;
    private readonly TimeSpan _jobFetchDelay;
    private readonly TimeSpan _loadTimeout;
    private readonly Random _random = new();

    private readonly ILogger _logger;
    private readonly IWebDriver _driver;
    private readonly LinkedInApi _linkedInApi;
    private readonly WebDriverWait _pageWait;

    public LinkedInScrapper(ILogger<LinkedInScrapper> logger, IConfiguration configuration)
    {
        _configuration = configuration.GetSection("LinkedIn");

        _maxFetchRetry = _configuration.GetValue("MaxFetchRetry", _maxFetchRetry);
        _maxPagesToScrape = _configuration.GetValue("MaxPagesToScrape", _maxPagesToScrape);
        _maxJobsToFetch = _configuration.GetValue("MaxJobsToFetch", _maxJobsToFetch);
        _runHeadless = _configuration.GetValue("RunHeadless", _runHeadless);

        _jobFetchDelay = TimeSpan.FromSeconds(_configuration.GetValue("JobFetchDelay", 2));
        _loadTimeout = TimeSpan.FromSeconds(_configuration.GetValue("LoadTimeoutSec", 60));

        _logger = logger;
        _driver = InitializeDriver();
        _pageWait = new WebDriverWait(_driver, _loadTimeout)
        {
            PollingInterval = TimeSpan.FromSeconds(1)
        };
        _linkedInApi = new(logger, _driver, _configuration);
    }

    public void Dispose()
    {
        try
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to quit driver");
        }
    }

    public string Platform => "LinkedIn";

    public async IAsyncEnumerable<JobDescription> GetJobs(
        DateTime lastScrapTime,
        JobSearchDescription jobSearch)
    {
        if (!await EnsureAuthentication())
        {
            _logger.LogError("Authentication failed, aborting scrape");
            yield break;
        }

        var baseUrl = jobSearch.BuildSearchUrl();
        var consecutiveEmptyPages = 0;

        var sw = Stopwatch.StartNew();
        for (var pageIndex = 0; pageIndex < _maxPagesToScrape; pageIndex++)
        {
            var pageUrl = BuildPagedUrl(baseUrl, pageIndex);
            if (!await NavigateToPage(pageUrl))
            {
                continue;
            }

            var jobs = await ProcessPageJobs(lastScrapTime, jobSearch, sw);
            foreach (var job in jobs)
            {
                yield return job;
            }

            if (jobs.Count == 0 && ++consecutiveEmptyPages >= 2)
            {
                break;
            }

            consecutiveEmptyPages = jobs.Count == 0 ? consecutiveEmptyPages : 0;
        }
    }
}
