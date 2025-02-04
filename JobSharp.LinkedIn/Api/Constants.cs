namespace JobSharp.LinkedIn.Api;

internal static class Constants
{
    public const int MaxJobsPerPage = 25;

    public const string LinkedInBaseUrl = "https://www.linkedin.com";
    public const string CookieDomain = ".www.linkedin.com";
    public const string LoginUrl = $"{LinkedInBaseUrl}/login";
    public const string JobBoardUrl = $"{LinkedInBaseUrl}/jobs/search/?sortBy=DD";

    public const string WorldWideGeoId = "92000000";
}
