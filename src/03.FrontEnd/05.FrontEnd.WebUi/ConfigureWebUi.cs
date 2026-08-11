using System.Globalization;
using System.Reflection;
using ApexCharts;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using EBVL.FrontEnd.WebUi.Common.Helpers;
using EBVL.FrontEnd.WebUi.Common.Services.Clipboard;
using EBVL.FrontEnd.WebUi.Common.Services.Preferences;
using EBVL.FrontEnd.WebUi.Layouts.Models;

namespace EBVL.FrontEnd.WebUi;

public static class ConfigureWebUi
{
    private const string EnglishUs = "en-US";
    private const string IndonesiaId = "id-ID";

    public static void AddWebUi(this WebApplicationBuilder builder)
    {
        var supportedCultures = new[]
        {
            new CultureInfo(EnglishUs),
            new CultureInfo(IndonesiaId)
        };

        _ = builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(EnglishUs);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new[]
            {
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });

        CultureInfo.DefaultThreadCurrentCulture = supportedCultures[0];
        CultureInfo.DefaultThreadCurrentUICulture = supportedCultures[0];

        _ = builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        _ = builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        _ = builder.Services.AddCascadingAuthenticationState();
        _ = builder.Services.AddScoped<ClipboardService>();
        _ = builder.Services.AddScoped<PreferencesService>();
        _ = builder.Services.AddCascadingValue(serviceProvider => CascadingValueHelper.CreateNotifying(new DisplayInfo()));
        _ = builder.Services.AddMudServices();

        _ = builder.Services.AddApexCharts(e =>
        {
            e.GlobalOptions = new ApexChartBaseOptions
            {
                Debug = false,
                Theme = new Theme { Palette = PaletteType.Palette4 }
            };
        });
    }
}
