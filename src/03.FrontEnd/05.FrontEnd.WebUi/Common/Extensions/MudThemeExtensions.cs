using System.Text.Json;

namespace EBVL.FrontEnd.WebUi.Common.Extensions;

public static class MudThemeExtensions
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static MudTheme Clone(this MudTheme theme)
    {
        var json = JsonSerializer.Serialize(theme, _options);

        return JsonSerializer.Deserialize<MudTheme>(json, _options)!;
    }
}
