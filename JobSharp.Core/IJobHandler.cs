using JobSharp.Core.Payload;

namespace JobSharp.Core;

public interface IJobHandler
{
    Task Handle(JobDescription job);
}
