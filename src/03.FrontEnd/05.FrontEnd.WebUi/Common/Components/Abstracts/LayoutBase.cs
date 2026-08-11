using EBVL.FrontEnd.WebUi.Common.Services.Preferences;
using EBVL.FrontEnd.WebUi.Layouts.Models;

namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class LayoutBase : LayoutComponentBase
{
    [Inject]
    public required IOptions<AppConfigFrontEndOptions> AppConfigFrontEndOptions { get; init; }

    [Inject]
    public required PreferencesService PreferencesService { get; init; }

    [CascadingParameter]
    public DisplayInfo DisplayInfo { get; set; } = default!;

    protected MudThemeProvider _mudThemeProvider = default!;
    private StoredPreferences _storedPreferences = new();

    protected override async Task OnInitializedAsync()
    {
        _storedPreferences = await PreferencesService.GetPreferencesAsync();

        DisplayInfo.IsDarkMode = _storedPreferences.IsDarkMode;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _mudThemeProvider.WatchSystemDarkModeAsync(OnSystemDarkModeChanged);
        }
    }

    private async Task OnSystemDarkModeChanged(bool isDarkMode)
    {
        DisplayInfo.IsDarkMode = isDarkMode;
        _storedPreferences.IsDarkMode = DisplayInfo.IsDarkMode;

        await PreferencesService.SetPreferencesAsync(_storedPreferences);

        StateHasChanged();
    }

    protected async Task ToggleDarkMode()
    {
        DisplayInfo.IsDarkMode = !DisplayInfo.IsDarkMode;
        _storedPreferences.IsDarkMode = DisplayInfo.IsDarkMode;

        await PreferencesService.SetPreferencesAsync(_storedPreferences);

        StateHasChanged();
    }
}
