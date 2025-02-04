using JobSharp.Core.Payload;
using JobSharp.LinkedIn.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace JobSharp.LinkedIn;

public partial class LinkedInScrapper
{
    /// <summary>
    /// Ensure that the user is authenticated
    /// </summary>
    /// <returns></returns>
    private async Task<bool> EnsureAuthentication()
    {
        if (_linkedInApi.IsAuthenticated())
        {
            return true;
        }

        _logger.LogInformation("Initiating authentication...");
        return await _linkedInApi.Authenticate(_configuration, CreateLinkedTimeoutToken());
    }

    /// <summary>
    /// Append to the base URL page offset
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    private string BuildPagedUrl(string baseUrl, int pageIndex)
    {
        return $"{baseUrl}&start={pageIndex * Constants.MaxJobsPerPage}";
    }

    /// <summary>
    /// Navigate to a page
    /// </summary>
    private async Task<bool> NavigateToPage(string url)
    {
        try
        {
            return await _linkedInApi.VisitPage(url, CreateLinkedTimeoutToken());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to navigate to page {PageUrl}", url);
            return false;
        }
    }

    /// <summary>
    /// Process jobs on a page
    /// </summary>
    private async Task<List<JobDescription>> ProcessPageJobs(
       DateTime lastScrapTime,
       JobSearchDescription jobSearch,
       Stopwatch timer)
    {
        var jobs = new List<JobDescription>();
        try
        {
            try
            {
                _pageWait.Until(d => d.FindElement(By.CssSelector(SelectorConstants.Container)));
            }
            catch
            {
                return jobs;
            }

            for (var jobIndex = 0; jobIndex < Constants.MaxJobsPerPage; jobIndex++)
            {
                if (jobs.Count >= _maxJobsToFetch) break;
                if (timer.Elapsed > _loadTimeout) break;

                var job = await FetchJobWithRetry(jobIndex, jobSearch);
                if (job is null || job.PostDate < lastScrapTime)
                {
                    break;
                }

                if (job.IsPromoted && jobSearch.SkipPromoted)
                {
                    continue;
                }

                jobs.Add(job);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing page jobs");
        }
        return jobs;
    }

    private async Task<JobDescription?> FetchJobWithRetry(
        int jobIndex,
        JobSearchDescription jobSearch)
    {
        for (var retry = 0; retry <= _maxFetchRetry; retry++)
        {
            try
            {
                await RandomizedDelay(retry);
                return await _linkedInApi.GetJobDetails(
                    jobIndex,
                    CreateLinkedTimeoutToken());
            }
            catch (Exception ex) when (retry < _maxFetchRetry)
            {
                _logger.LogWarning(ex, "Retry {RetryCount} for job index {JobIndex}", retry, jobIndex);
            }
        }
        return null;
    }

    private async Task RandomizedDelay(int retryCount)
    {
        var factor = Math.Pow(1.5, retryCount);
        var maxDelay = _jobFetchDelay.TotalMilliseconds * factor;
        var delayMs = _random.Next(
            (int)(_jobFetchDelay.TotalMilliseconds / 2),
            (int)maxDelay);

        await Task.Delay(TimeSpan.FromMilliseconds(delayMs));
    }

    private ChromeDriver InitializeDriver()
    {
        var options = new ChromeOptions();

        if (_runHeadless)
        {
            // Headless configuration
            options.AddArgument("--headless=new"); // New headless mode in Chrome 109+
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
        }

        // Anti-detection features
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddAdditionalOption("useAutomationExtension", false);

        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(1);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(1);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(1);

        return driver;
    }

    private CancellationToken CreateLinkedTimeoutToken() =>
        CreateTimeoutTokenSource().Token;

    private CancellationTokenSource CreateTimeoutTokenSource() =>
        new CancellationTokenSource(_loadTimeout);
}
