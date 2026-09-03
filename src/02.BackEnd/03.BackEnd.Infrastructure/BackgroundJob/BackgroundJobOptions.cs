namespace EBVL.BackEnd.Infrastructure.BackgroundJob;

public sealed class BackgroundJobOptions
{
    public const string SectionKey = "BackgroundJob";

    public string DashboardUrl { get; init; } = "/hf";

    public int WorkerCount { get; init; }
}
