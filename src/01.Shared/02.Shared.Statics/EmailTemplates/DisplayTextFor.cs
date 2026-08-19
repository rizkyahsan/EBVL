using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.EmailTemplates;

public static class DisplayTextFor
{
    public static readonly string EmailTemplate = nameof(EmailTemplate).SplitWords();
    public static readonly string EmailTemplates = nameof(EmailTemplates).SplitWords();

    public const string Module = nameof(Module);
    public const string Action = nameof(Action);
    public const string To = nameof(To);
    public static readonly string DefaultTo = nameof(DefaultTo).SplitWords();
    public const string Cc = nameof(Cc);
    public static readonly string DefaultCc = nameof(DefaultCc).SplitWords();
    public const string Subject = nameof(Subject);
    public const string Content = nameof(Content);
}
