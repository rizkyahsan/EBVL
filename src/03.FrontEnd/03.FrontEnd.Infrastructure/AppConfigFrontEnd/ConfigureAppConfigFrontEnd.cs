using EBVL.FrontEnd.Services.AppConfigFrontEnd;

namespace EBVL.FrontEnd.Infrastructure.AppConfigFrontEnd;

public static class ConfigureAppConfigFrontEnd
{
    public static AppConfigFrontEndOptions GetAppConfigFrontEndOptions(this WebApplicationBuilder builder)
    {
        var appConfigFrontEndSection = builder.Configuration.GetRequiredSection(AppConfigFrontEndOptions.SectionKey);
        var appConfigFrontEndOptions = appConfigFrontEndSection.Get<AppConfigFrontEndOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(AppConfigFrontEndOptions.SectionKey, typeof(AppConfigFrontEndOptions));

        _ = builder.Services.Configure<AppConfigFrontEndOptions>(appConfigFrontEndSection);

        return appConfigFrontEndOptions;
    }
}
