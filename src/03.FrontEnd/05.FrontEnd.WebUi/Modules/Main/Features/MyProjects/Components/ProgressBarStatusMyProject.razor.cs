using EBVL.Shared.Statics.Statuses;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Components;

public partial class ProgressBarStatusMyProject
{
    [Parameter]
    public required string Width { get; init; }

    [Parameter]
    public required int ColspanProgressBar { get; init; }

    [Parameter]
    public required string ProjectStatusCode { get; init; }

    [Parameter]
    public required string ProjectLenderStatusCode { get; init; }

    [Parameter]
    public required IDictionary<Guid, MyProjectStageItem> ListStage { get; init; }

    private enum ProjectState
    {
        InProgress,
        Review,
        Completed,
        LenderLost,
        Cancelled
    }
    private string ResultText => ProjectLenderStatusCode switch
    {
        CodeFor.ProjectLenderWin => "Win",
        CodeFor.ProjectLenderLose => "Lose",
        _ => "Result"
    };

    private MyProjectStageItem? CurrentStage =>
        ListStage.Values
        .LastOrDefault(x => x.StatusCode is not CodeFor.ProjectStageComplete and not CodeFor.ProjectStageCancelDelete);

    private HashSet<Guid> CompletedStages =>
        [.. ListStage.Values.TakeWhile(x => x.StatusCode != CurrentStage?.StatusCode).Select(x => x.Id)];

    private ProjectState State
    {
        get
        {
            if (ProjectStatusCode == CodeFor.ProjectCancel)
            {
                return ProjectState.Cancelled;
            }

            if (ProjectLenderStatusCode == CodeFor.ProjectLenderLose)
            {
                return ProjectState.LenderLost;
            }

            if (ProjectStatusCode == CodeFor.ProjectComplete)
            {
                return ProjectState.Completed;
            }

            if (CurrentStage?.StatusCode == CodeFor.ProjectStageOnReview)
            {
                return ProjectState.Review;
            }

            return ProjectState.InProgress;
        }
    }

    private double ProgressPercentage
    {
        get
        {
            var step = 100d / ColspanProgressBar;

            return State switch
            {
                ProjectState.InProgress => CurrentStageProgress(),
                ProjectState.Review => CurrentStageProgress(),
                ProjectState.Completed => 100,
                ProjectState.LenderLost => CurrentStageProgress(),
                ProjectState.Cancelled => 100,
                _ => step / 2
            };
        }
    }

    private double CurrentStageProgress()
    {
        var step = 100d / ColspanProgressBar;

        if (CurrentStage is null)
        {
            return ListStage.Count * step;
        }

        var index = ListStage.Values.ToList().IndexOf(CurrentStage);

        return (index * step) + (step / 2);
    }

    private Color GetStageColor(MyProjectStageItem stage)
    {
        if (State == ProjectState.LenderLost)
        {
            if (CompletedStages.Contains(stage.Id) ||
                stage.Id == CurrentStage?.Id)
            {
                return Color.Error;
            }

            return Color.Default;
        }

        if (CompletedStages.Contains(stage.Id))
        {
            return Color.Success;
        }

        if (stage.Id != CurrentStage?.Id)
        {
            return Color.Default;
        }

        return State switch
        {
            ProjectState.InProgress => Color.Info,
            ProjectState.Review => Color.Info,
            ProjectState.Completed => Color.Success,
            ProjectState.Cancelled => Color.Error,
            ProjectState.LenderLost => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetResultColor()
    {
        if (ProjectStatusCode != CodeFor.ProjectComplete)
        {
            return State switch
            {
                ProjectState.InProgress => Color.Default,
                ProjectState.Review => Color.Default,
                ProjectState.Completed => Color.Default,
                ProjectState.LenderLost => Color.Default,
                ProjectState.Cancelled => Color.Error,
                _ => Color.Default
            };
        }

        return ProjectLenderStatusCode switch
        {
            CodeFor.ProjectLenderWin => Color.Success,
            CodeFor.ProjectLenderLose => Color.Error,
            _ => Color.Success
        };
    }

    private Color GetProgressColor()
    {
        if (ProjectStatusCode == CodeFor.ProjectComplete)
        {
            return ProjectLenderStatusCode switch
            {
                CodeFor.ProjectLenderWin => Color.Success,
                CodeFor.ProjectLenderLose => Color.Error,
                _ => Color.Success
            };
        }

        return State switch
        {
            ProjectState.InProgress => Color.Primary,
            ProjectState.Review => Color.Primary,
            ProjectState.LenderLost => Color.Error,
            ProjectState.Cancelled => Color.Error,
            ProjectState.Completed => Color.Primary,
            _ => Color.Primary
        };
    }
}
