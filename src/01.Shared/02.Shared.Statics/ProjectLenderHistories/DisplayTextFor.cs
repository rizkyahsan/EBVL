using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectLenderHistories;

public static class DisplayTextFor
{
    public static readonly string ProjectLenderHistory = nameof(ProjectLenderHistory).SplitWords();
    public static readonly string ProjectLenderHistories = nameof(ProjectLenderHistories).SplitWords();

    public const string Remarks = nameof(Remarks);
}
