namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Emails.Statics;

public static class RouteFor
{
    public const string SendEmail = $"{ExamplesRouteFor.Index}/{nameof(Emails)}/{nameof(SendEmail)}";
    public const string SendEmailWithTemplate = $"{ExamplesRouteFor.Index}/{nameof(Emails)}/{nameof(SendEmailWithTemplate)}";
}
