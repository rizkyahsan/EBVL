using EBVL.Shared.Statics.Statuses;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Components;

public partial class ProgressBarStatusProject
{
    [Parameter]
    public required string Width { get; init; }

    [Parameter]
    public required int ColspanProgressBar { get; init; }

    [Parameter]
    public required string ProjectStatusCode { get; init; }

    [Parameter]
    public required IDictionary<Guid, ProjectStageItem> ListStage { get; init; }

    private enum ProjectState
    {
        Draft,
        OnProgress,
        Review,
        Completed,
        Cancelled
    }

    private ProjectStageItem? CurrentStage =>
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

            if (ProjectStatusCode == CodeFor.ProjectComplete)
            {
                return ProjectState.Completed;
            }

            if (ProjectStatusCode == CodeFor.ProjectDraft)
            {
                return ProjectState.Draft;
            }

            if (CurrentStage?.StatusCode == CodeFor.ProjectStageOnReview)
            {
                return ProjectState.Review;
            }

            return ProjectState.OnProgress;
        }
    }

    private double ProgressPercentage
    {
        get
        {
            var step = 100d / ColspanProgressBar;

            return State switch
            {
                ProjectState.Draft => step / 2,
                ProjectState.OnProgress => CurrentStageProgress(),
                ProjectState.Review => CurrentStageProgress(),
                ProjectState.Completed => 100,
                ProjectState.Cancelled => 100,
                _ => 0
            };
        }
    }

    private double CurrentStageProgress()
    {
        var step = 100d / ColspanProgressBar;

        if (CurrentStage is null)
        {
            return (ListStage.Count + 1) * step;
        }

        var index = ListStage.Values.ToList().IndexOf(CurrentStage);

        return ((index + 1) * step) + (step / 2);
    }

    private Color GetDraftColor()
    {
        return State switch
        {
            ProjectState.Draft => Color.Info,
            ProjectState.OnProgress => Color.Success,
            ProjectState.Review => Color.Success,
            ProjectState.Completed => Color.Success,
            ProjectState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetStageColor(ProjectStageItem stage)
    {
        if (CompletedStages.Contains(stage.Id))
        {
            return Color.Success;
        }

        if (stage.StatusCode != CurrentStage?.StatusCode)
        {
            return Color.Default;
        }

        return State switch
        {
            ProjectState.Draft => Color.Default,
            ProjectState.OnProgress => Color.Info,
            ProjectState.Review => Color.Info,
            ProjectState.Completed => Color.Success,
            ProjectState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetResultColor()
    {
        return State switch
        {
            ProjectState.Draft => Color.Default,
            ProjectState.OnProgress => Color.Default,
            ProjectState.Review => Color.Default,
            ProjectState.Completed => Color.Success,
            ProjectState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetProgressColor()
    {
        return State switch
        {
            ProjectState.Draft => Color.Primary,
            ProjectState.OnProgress => Color.Primary,
            ProjectState.Review => Color.Primary,
            ProjectState.Completed => Color.Success,
            ProjectState.Cancelled => Color.Error,
            _ => Color.Primary
        };
    }
}
