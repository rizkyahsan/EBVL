using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EBVL.FrontEnd.WebUi.Common.Services.Preferences;

public sealed class PreferencesService(ProtectedLocalStorage protectedLocalStorage)
{
    public async Task<StoredPreferences> GetPreferencesAsync()
    {
        try
        {
            var result = await protectedLocalStorage.GetAsync<StoredPreferences>(StoredPreferences.Key);

            if (result.Success && result.Value is not null)
            {
                return result.Value;
            }
        }
        catch (CryptographicException)
        {
            // Old encryption key no longer exists.
            await protectedLocalStorage.DeleteAsync(StoredPreferences.Key);
        }

        return new StoredPreferences();
    }

    public async Task SetPreferencesAsync(StoredPreferences preferences)
    {
        await protectedLocalStorage.SetAsync(StoredPreferences.Key, preferences);
    }
}
