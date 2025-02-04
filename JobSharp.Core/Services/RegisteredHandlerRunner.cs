using JobSharp.Core.Payload;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace JobSharp.Core.Services;

internal static class RegisteredHandlerRunner
{
    public static async Task Run(
        IReadOnlyList<IJobHandler> handlers,
        ConcurrentBag<JobDescription> jobs,
        ILogger logger)
    {
        var handlerTasks = handlers.Select(handler =>
            ProcessHandlerAsync(handler, jobs, logger));

        await Task.WhenAll(handlerTasks);
    }

    private static async Task ProcessHandlerAsync(
        IJobHandler handler,
        ConcurrentBag<JobDescription> jobs,
        ILogger logger)
    {
        var jobTasks = jobs.Select(job =>
            ProcessSingleJobAsync(handler, job, logger));

        await Task.WhenAll(jobTasks);
    }

    private static async Task ProcessSingleJobAsync(
        IJobHandler handler,
        JobDescription job,
        ILogger logger)
    {
        try
        {
            await handler.Handle(job);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to handle job {JobId} with handler {HandlerName}",
                job.JobId, handler.GetType().Name);
        }
    }
}
