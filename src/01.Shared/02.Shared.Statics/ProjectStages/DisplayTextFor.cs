using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectStages;

public static class DisplayTextFor
{
    public static readonly string ProjectStage = nameof(ProjectStage).SplitWords();
    public static readonly string ProjectStages = nameof(ProjectStages).SplitWords();

    public static readonly string MyProjectStage = nameof(MyProjectStage).SplitWords();
    public static readonly string MyProjectStages = nameof(MyProjectStages).SplitWords();

    public const string Stage = nameof(Stage);
    public const string Level = nameof(Level);
    public const string Name = nameof(Name);
    public const string Desc = nameof(Desc);
    public static readonly string DueDate = nameof(DueDate).SplitWords();
    public static readonly string TimesReminder = nameof(TimesReminder).SplitWords();
}
