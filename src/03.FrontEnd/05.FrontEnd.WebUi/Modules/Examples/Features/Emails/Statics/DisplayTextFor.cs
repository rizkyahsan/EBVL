namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Emails.Statics;

public static class DisplayTextFor
{
    public const string Emails = nameof(Emails);
    public const string Email = nameof(Email);
    public const string SendEmail = $"{CommonDisplayTextFor.Send} {Email}";
    public const string SendEmailWithTemplate = $"{SendEmail} with Template";
}
