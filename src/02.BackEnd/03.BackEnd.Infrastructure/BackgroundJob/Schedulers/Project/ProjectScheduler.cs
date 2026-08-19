using EBVL.BackEnd.Logics.Modules.Main.Projects.CheckExpiredProjectStage;
using MediatR;

namespace EBVL.BackEnd.Infrastructure.BackgroundJob.Schedulers.Project;

public sealed class ProjectScheduler(ISender sender) : IProjectScheduler
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await sender.Send(
            new CheckExpiredProjectStageCommand(),
            cancellationToken);
    }
}
