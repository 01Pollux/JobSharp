namespace JobSharp.Core.Payload;

public record JobDescription(
    string JobId,
    string Title,
    string Description,
    string Company,
    string CompanyLink,
    string CompanyImageLink,
    string Location,
    string Url,
    DateTime PostDate,
    bool IsPromoted);
