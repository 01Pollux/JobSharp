namespace JobSharp.LinkedIn.Api;

public record LinkedInJobDescription(
    string JobId,
    string Title,
    string Description,
    string Company,
    string CompanyLink,
    string CompanyImageLink,
    string Location,
    string Url,
    string PostDate,
    bool IsPromoted);
