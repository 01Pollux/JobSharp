using OpenQA.Selenium;
using System.Text.Json;

namespace JobSharp.LinkedIn.Api;

internal static class LinkedInApiScripts
{
    public static async Task<LinkedInJobDescription?> FetchJobDetails(IJavaScriptExecutor jsExecutor, int index, CancellationToken ct)
    {
        var jobDetailsScript = JsScriptConstants.FetchJobDetails;
        var jobDetailsResult = await Task.Run(() => jsExecutor.ExecuteScript(jobDetailsScript, index) as string, ct);
        return jobDetailsResult is not null ? JsonSerializer.Deserialize<LinkedInJobDescription>(jobDetailsResult) : null;
    }
}
