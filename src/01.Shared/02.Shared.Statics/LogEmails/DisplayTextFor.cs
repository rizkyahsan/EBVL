using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.LogEmails;

public static class DisplayTextFor
{
    public static readonly string LogEmail = nameof(LogEmail).SplitWords();
    public static readonly string LogEmails = nameof(LogEmails).SplitWords();

    public const string Provider = nameof(Provider);
    public const string Module = nameof(Module);
    public const string Action = nameof(Action);
    public const string To = nameof(To);
    public const string Cc = nameof(Cc);
    public const string Subject = nameof(Subject);
    public const string Content = nameof(Content);
    public static readonly string SentAt = nameof(SentAt).SplitWords();
    public static readonly string IsSuccessful = nameof(IsSuccessful).SplitWords();
    public static readonly string RetryCount = nameof(RetryCount).SplitWords();
    public static readonly string ErrorMessage = nameof(ErrorMessage).SplitWords();
}
