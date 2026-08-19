using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.Projects;

public static class DisplayTextFor
{
    public const string Project = nameof(Project);
    public const string Projects = nameof(Projects);

    public static readonly string MyProject = nameof(MyProject).SplitWords();
    public static readonly string MyProjects = nameof(MyProjects).SplitWords();

    public const string Title = nameof(Title);
    public const string Desc = "Description";
    public const string Objective = nameof(Objective);
    public const string FinanceType = "Type of Financing";
}
