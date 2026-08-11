using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EBVL.FrontEnd.WebUi.Common.Services.Preferences;

public sealed class PreferencesService(ProtectedLocalStorage protectedLocalStorage)
{
    public async Task<StoredPreferences> GetPreferencesAsync()
    {
        var result = await protectedLocalStorage.GetAsync<StoredPreferences>(StoredPreferences.Key);

        return result.Value ?? new StoredPreferences();
    }

    public async Task SetPreferencesAsync(StoredPreferences preferences)
    {
        await protectedLocalStorage.SetAsync(StoredPreferences.Key, preferences);
    }
}
