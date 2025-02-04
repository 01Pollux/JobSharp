using JobSharp.Core.Payload;
using System.Text.RegularExpressions;
using System.Web;

namespace JobSharp.LinkedIn.Api;

public static class LinkedInMapper
{
    public static JobDescription ToJobDescription(this LinkedInJobDescription jobDescription)
    {
        return new JobDescription(
            jobDescription.JobId,
            jobDescription.Title,
            jobDescription.Description,
            jobDescription.Company,
            jobDescription.CompanyLink,
            jobDescription.CompanyImageLink,
            jobDescription.Location,
            jobDescription.Url,
            ConvertRelativeDate(jobDescription.PostDate),
            jobDescription.IsPromoted);
    }

    public static string BuildSearchUrl(this JobSearchDescription search)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);

        // Add search term
        if (!string.IsNullOrEmpty(search.SearchTerm))
        {
            queryParams.Add("keywords", search.SearchTerm);
        }

        // Add location
        if (!string.IsNullOrEmpty(search.Location))
        {
            queryParams.Add("location", search.Location);
        }
        else
        {
            queryParams.Add("geoId", Constants.WorldWideGeoId);
        }

        // Add experience level filter (f_E)
        var experienceCode = GetExperienceLevelCode(search.ExperienceLevel);
        if (experienceCode is not null)
        {
            queryParams.Add("f_E", experienceCode);
        }

        // Add remote type filter (f_WT)
        var remoteCode = GetRemoteTypeCode(search.RemoteType);
        if (remoteCode is not null)
        {
            queryParams.Add("f_WT", remoteCode);
        }

        return $"{Constants.JobBoardUrl}&{queryParams}";
    }

    private static string? GetExperienceLevelCode(JobExperienceLevel level)
    {
        return level switch
        {
            JobExperienceLevel.Intern => "1",
            JobExperienceLevel.Entry => "2",
            JobExperienceLevel.Associate => "3",
            JobExperienceLevel.Senior => "4",
            JobExperienceLevel.Director => "5",
            JobExperienceLevel.Executive => "6",
            _ => null
        };
    }

    private static string? GetRemoteTypeCode(JobRemoteType remoteType)
    {
        return remoteType switch
        {
            JobRemoteType.OnSite => "1",
            JobRemoteType.Remote => "2",
            JobRemoteType.Hybrid => "3",
            _ => null
        };
    }

    private static DateTime ConvertRelativeDate(string relativeDate)
    {
        var cleanDate = relativeDate
            .Replace("Posted", "")
            .Replace("about", "")
            .Trim();

        var number = int.Parse(Regex.Match(cleanDate, @"\d+").Value);

        if (cleanDate.Contains("second")) return DateTime.Now.AddSeconds(-number);
        if (cleanDate.Contains("minute")) return DateTime.Now.AddMinutes(-number);
        if (cleanDate.Contains("hour")) return DateTime.Now.AddHours(-number);
        if (cleanDate.Contains("day")) return DateTime.Now.Date.AddDays(-number);
        if (cleanDate.Contains("week")) return DateTime.Now.Date.AddDays(-number * 7);
        if (cleanDate.Contains("month")) return DateTime.Now.Date.AddMonths(-number);

        return DateTime.Now;
    }
}
