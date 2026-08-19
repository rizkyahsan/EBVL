namespace EBVL.BackEnd.Infrastructure.BackgroundJob.Schedulers.Project;

public interface IProjectScheduler
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default);
}
