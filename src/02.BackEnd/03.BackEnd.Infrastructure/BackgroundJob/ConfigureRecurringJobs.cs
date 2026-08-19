using EBVL.BackEnd.Infrastructure.BackgroundJob.Constants;
using EBVL.BackEnd.Infrastructure.BackgroundJob.Schedulers.Project;
using Hangfire;

namespace EBVL.BackEnd.Infrastructure.BackgroundJob;

public static class ConfigureRecurringJobs
{
    public static void RegisterRecurringJobs(this WebApplication app)
    {
        var options = app.Configuration
            .GetRequiredSection(BackgroundJobOptions.SectionKey)
            .Get<BackgroundJobOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(
                BackgroundJobOptions.SectionKey,
                typeof(BackgroundJobOptions));

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.TimeZone);

        RecurringJob.AddOrUpdate<IProjectScheduler>(
            recurringJobId: BackgroundJobId.ProjectExpired,
            methodCall: x => x.ExecuteAsync(default),
            cronExpression: options.ProjectExpiredCron,
            options: new RecurringJobOptions
            {
                TimeZone = timeZone
            });

        // Future jobs
        /*
        RecurringJob.AddOrUpdate<IEmailScheduler>(...);

        RecurringJob.AddOrUpdate<INotificationScheduler>(...);
        */
    }
}
