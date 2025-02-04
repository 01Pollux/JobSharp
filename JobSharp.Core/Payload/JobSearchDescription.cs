namespace JobSharp.Core.Payload;

public record JobSearchDescription(
    string SearchTerm, 
    string Location = "",
    JobExperienceLevel ExperienceLevel = JobExperienceLevel.Any,
    JobRemoteType RemoteType = JobRemoteType.Any,
    bool SkipPromoted = true);
