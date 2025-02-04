using JobSharp.Core.Payload;

namespace JobSharp.Core.Handlers;

public class PrintToConsoleJobHandler : IJobHandler
{
    public Task Handle(JobDescription job)
    {
        Console.WriteLine($"Job ID: {job.JobId}");
        Console.WriteLine($"Title: {job.Title}");
        Console.WriteLine($"Description: {job.Description}");
        Console.WriteLine($"Company: {job.Company}");
        Console.WriteLine($"CompanyLink: {job.CompanyLink}");
        Console.WriteLine($"CompanyImageLink: {job.CompanyImageLink}");
        Console.WriteLine($"Location: {job.Location}");
        Console.WriteLine($"Url: {job.Url}");
        Console.WriteLine($"PostDate: {job.PostDate}");
        Console.WriteLine($"IsPromoted: {job.IsPromoted}");

        Console.WriteLine();
        return Task.CompletedTask;
    }
}
