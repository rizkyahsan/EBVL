using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectReqs;

public static class DisplayTextFor
{
    public static readonly string ProjectReq = nameof(ProjectReq).SplitWords();
    public static readonly string ProjectReqs = nameof(ProjectReqs).SplitWords();
    public static readonly string ProjectReqlabel = "Document Requirement";

    public const string Name = nameof(Name);
    public const string Desc = nameof(Desc);
    public const string SortNo = "View Order";
    public static readonly string IsRequired = nameof(IsRequired).SplitWords();
    public const string File = nameof(File);
}
