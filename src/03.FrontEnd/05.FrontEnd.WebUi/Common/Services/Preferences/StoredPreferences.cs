namespace EBVL.FrontEnd.WebUi.Common.Services.Preferences;

public sealed record StoredPreferences
{
    public const string Key = $"{nameof(EBVL)}.{nameof(StoredPreferences)}";

    public bool IsDarkMode { get; set; }
}
