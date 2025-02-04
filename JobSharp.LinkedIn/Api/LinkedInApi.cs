using JobSharp.Core;
using JobSharp.Core.Payload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;

namespace JobSharp.LinkedIn.Api;

internal partial class LinkedInApi
{
    private readonly ILogger _logger;
    private readonly ChromeDriver _driver;
    private readonly IJavaScriptExecutor _jsExecutor;

    private readonly WebDriverWait _pageWait;
    private readonly TimeSpan _pageLoadDelay;

    public LinkedInApi(ILogger logger, IWebDriver driver, IConfiguration configuration)
    {
        _logger = logger;
        _driver = (ChromeDriver)driver;
        _jsExecutor = (IJavaScriptExecutor)driver;

        ArgumentNullException.ThrowIfNull(_driver, "Driver not found");
        ArgumentNullException.ThrowIfNull(_jsExecutor, "JavaScript executor not found");

        _logger.LogTrace("LinkedIn API initialized");

        _pageLoadDelay = TimeSpan.FromSeconds(configuration.GetValue("PageLoadDelayInSec", 1));

        _pageWait = new WebDriverWait(_driver, _pageLoadDelay * 2)
        {
            PollingInterval = TimeSpan.FromMilliseconds(500)
        };
        _pageWait.IgnoreExceptionTypes(
            typeof(StaleElementReferenceException),
            typeof(NoSuchElementException));
    }

    //

    /// <summary>
    /// Visit a page and wait for it to load
    /// </summary>
    public async Task<bool> VisitPage(string url, CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Navigation: {Url}", url);

        try
        {
            _logger.LogDebug("Initiating navigation");
            _driver.Navigate().GoToUrl(url);

            await WaitForPageReady(ct);
            return true;
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            _logger.LogError(e, "Navigation failed");
            return false;
        }
    }

    /// <summary>
    /// Visit the main LinkedIn page
    /// </summary>
    public Task<bool> LoadMainPage(CancellationToken ct)
    {
        return VisitPage(Constants.LinkedInBaseUrl, ct);
    }

    //

    /// <summary>
    /// Check if the user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        var cookie = _driver.Manage().Cookies.GetCookieNamed("li_at");
        return cookie is not null;
    }

    /// <summary>
    /// Login to LinkedIn with username and password
    /// </summary>
    public async Task<bool> Authenticate(IConfiguration config, CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Authentication");

        return await TryUseCookieAuth(config, ct) ||
               await TryCredentialsAuth(config, ct);
    }

    /// <summary>
    /// Add authentication cookie
    /// </summary>
    public async Task<bool> AddCookie(string cookie, CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Adding cookie");
        try
        {
            if (!await LoadMainPage(ct))
            {
                return false;
            }

            _logger.LogTrace("Adding cookie");
            _driver.Manage().Cookies.AddCookie(new Cookie("li_at", cookie, Constants.CookieDomain, "/", DateTime.Now.AddYears(1)));

            return IsAuthenticated();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add cookie");
        }
        return false;
    }

    //

    /// <summary>
    /// Wait for the job board container to load
    /// </summary>
    public bool WaitForJobBoardContainer(WebDriverWait driverWait)
    {
        try
        {
            driverWait.Until(d => d.FindElement(By.CssSelector(SelectorConstants.Container)));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to wait for job board container");
        }
        return false;
    }

    /// <summary>
    /// Selects a job by index
    /// </summary>
    /// <returns>
    /// The job description or null if the job could not be selected (index out of bounds)
    /// </returns>
    /// <exception>
    /// <see cref="Exception">Thrown when the javascript executor fails to execute the script</see>
    /// <see cref="TaskCanceledException">Thrown when the task is cancelled</see>
    /// <see cref="ArgumentNullException">Thrown when the job description is null</see>
    /// <see cref="JsonException">Thrown when the job description cannot be deserialized</see>
    /// </exception>
    public async Task<JobDescription?> GetJobDetails(int index, CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Getting job {Index}", index);

        var jobJson = await LinkedInApiScripts.FetchJobDetails(_jsExecutor, index, ct);
        return jobJson?.ToJobDescription();
    }
}
