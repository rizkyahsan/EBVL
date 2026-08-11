using EBVL.BackEnd.Services.AppConfigBackEnd;

namespace EBVL.BackEnd.Infrastructure.AppConfigBackEnd;

public static class ConfigureAppConfigBackEnd
{
    public static AppConfigBackEndOptions GetAppConfigBackEndOptions(this WebApplicationBuilder builder)
    {
        var appConfigBackEndSection = builder.Configuration.GetRequiredSection(AppConfigBackEndOptions.SectionKey);
        var appConfigBackEndOptions = appConfigBackEndSection.Get<AppConfigBackEndOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(AppConfigBackEndOptions.SectionKey, typeof(AppConfigBackEndOptions));

        _ = builder.Services.Configure<AppConfigBackEndOptions>(appConfigBackEndSection);

        return appConfigBackEndOptions;
    }
}
