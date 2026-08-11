using System.Diagnostics;
using OpenTelemetry;

namespace EBVL.FrontEnd.Infrastructure.Monitoring;

public sealed class ActivityFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        if (activity.DisplayName.Contains("Microsoft.AspNetCore.Components.Server.ComponentHub"))
        {
            activity.IsAllDataRequested = false;
        }
    }
}
