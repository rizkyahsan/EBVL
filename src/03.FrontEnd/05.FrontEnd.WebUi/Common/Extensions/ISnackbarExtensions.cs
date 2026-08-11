using Severity = MudBlazor.Severity;

namespace EBVL.FrontEnd.WebUi.Common.Extensions;

public static class ISnackbarExtensions
{
    public static void AddSuccess(this ISnackbar snackbar, string message)
    {
        _ = snackbar.Add(message, Severity.Success);
    }

    public static void AddInfo(this ISnackbar snackbar, string message)
    {
        _ = snackbar.Add(message, Severity.Info);
    }

    public static void AddWarning(this ISnackbar snackbar, string message)
    {
        _ = snackbar.Add(message, Severity.Warning);
    }

    public static void AddWarnings(this ISnackbar snackbar, IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            _ = snackbar.Add(message, Severity.Warning);
        }
    }

    public static void AddError(this ISnackbar snackbar, string message)
    {
        _ = snackbar.Add(message, Severity.Error);
    }

    public static void AddErrors(this ISnackbar snackbar, IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            _ = snackbar.Add(message, Severity.Error);
        }
    }
}
