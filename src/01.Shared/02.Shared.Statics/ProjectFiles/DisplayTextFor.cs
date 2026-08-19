using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectFiles;

public static class DisplayTextFor
{
    public static readonly string ProjectFile = nameof(ProjectFile).SplitWords();
    public static readonly string ProjectFiles = nameof(ProjectFiles).SplitWords();

    public static readonly string FileName = nameof(FileName);
}
