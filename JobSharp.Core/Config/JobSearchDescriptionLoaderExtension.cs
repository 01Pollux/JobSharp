using JobSharp.Core.Payload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobSharp.Core.Config;

public static class JobSearchDescriptionLoaderExtension
{
    public static void AddJobSearchDescription(this IServiceCollection services, IConfiguration configuration, string sectionName = "Jobs")
    {
        var jobSearchDescriptions = configuration.GetSection(sectionName);
        if (!jobSearchDescriptions.Exists())
        {
            return;
        }

        var jobSearchDescriptionsList = jobSearchDescriptions.Get<List<JobSearchDescription>>();
        if (jobSearchDescriptionsList == null)
        {
            return;
        }

        foreach (var jobSearchDescription in jobSearchDescriptionsList)
        {
            services.AddSingleton(jobSearchDescription);
        }
    }
}
