using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectLenders;

public static class DisplayTextFor
{
    public static readonly string ProjectLender = nameof(ProjectLender).SplitWords();
    public static readonly string ProjectLenders = nameof(ProjectLenders).SplitWords();
    public static readonly string ProjectLenderLabel = "Lender Participants";

    public const string Lender = nameof(Lender);
    public const string Name = nameof(Name);
    public const string File = nameof(File);
    public const string Note = nameof(Note);
}
