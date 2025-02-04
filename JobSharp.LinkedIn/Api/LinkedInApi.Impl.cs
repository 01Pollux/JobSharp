using JobSharp.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace JobSharp.LinkedIn.Api;

internal partial class LinkedInApi
{
    /// <summary>
    /// Visit a page and wait for it to load
    /// </summary>
    private async Task WaitForPageReady(CancellationToken ct)
    {
        try
        {
            await Task.Run(() =>
                _pageWait.Until(d =>
                    (bool)_jsExecutor.ExecuteScript("return document.readyState === 'complete'")),
                ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Page load interrupted");
            throw;
        }
    }

    /// <summary>
    /// Try to authenticate using cookie
    /// </summary>
    private async Task<bool> TryUseCookieAuth(IConfiguration config, CancellationToken ct)
    {
        var cookie = config["Cookie"];
        if (string.IsNullOrEmpty(cookie))
        {
            return false;
        }

        try
        {
            _driver.Manage().Cookies.AddCookie(new(
                name: "li_at",
                value: cookie,
                domain: Constants.CookieDomain,
                path: "/",
                expiry: DateTime.Now.AddYears(1)));

            await VisitPage(Constants.LinkedInBaseUrl, ct);
            return IsAuthenticated();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Cookie authentication failed");
            return false;
        }
    }

    /// <summary>
    /// Try to authenticate using credentials
    /// </summary>
    private async Task<bool> TryCredentialsAuth(IConfiguration config, CancellationToken ct)
    {
        var email = config["Email"];
        var password = config["Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        try
        {
            await VisitPage(Constants.LoginUrl, ct);

            var loginWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            loginWait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            EnterCredentials(loginWait, email, password);
            SubmitLogin(loginWait);

            HandleTwoFactorAuthIfNeeded();
            VerifyLoginSuccess(loginWait);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Credential authentication failed");
            return false;
        }
    }

    private void EnterCredentials(WebDriverWait loginWait, string email, string password)
    {
        loginWait.Until(d => d.FindElement(By.Id("username")));
        _driver.FindElement(By.Id("username"))!.SendKeys(email);

        loginWait.Until(d => d.FindElement(By.Id("password")));
        JobUtils.HumanLikeTyping(_driver.FindElement(By.Id("password")), password);
    }

    private void SubmitLogin(WebDriverWait loginWait)
    {
        loginWait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
        _driver.FindElement(By.CssSelector("button[type='submit']"))!.Click();
    }

    private void HandleTwoFactorAuthIfNeeded()
    {
        if (_driver.Url.Contains("checkpoint/challenge"))
        {
            _logger.LogWarning("Manual 2FA intervention required!");
            Console.ReadKey(); // Pause for manual input
        }
    }

    private void VerifyLoginSuccess(WebDriverWait loginWait)
    {
        loginWait.Until(d => d.Url.Contains("linkedin.com/feed"));
    }
}
