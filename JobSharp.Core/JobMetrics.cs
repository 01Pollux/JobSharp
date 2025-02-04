namespace JobSharp.Core;

public record JobMetrics(
    int NewJobs,
    int SkippedJobs,
    int FailedJobs)
{
    public int TotalJobs => NewJobs + SkippedJobs + FailedJobs;
}
