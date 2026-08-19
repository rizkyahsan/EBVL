using EBVL.Shared.Statics.Statuses;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Components;

public partial class ProgressBarStatusMyProjectStage
{
    [Parameter]
    public required int StageLevel { get; init; }
    [Parameter]
    public required string ProjectStageStatusCode { get; init; }

    [Parameter]
    public required string ProjectLenderReqStatusCode { get; init; }

    private string StepWidth => StageLevel == 0 ? "25%" : "33.33%";
    private ProgressState _state;

    private enum ProgressState
    {
        OnProgress,
        Submit,
        Revision,
        Review,
        Accepted,
        Rejected,
        Cancelled
    }

    private ProgressState GetState()
    {
        // Cancelled has highest priority
        if (ProjectStageStatusCode == CodeFor.ProjectStageCancelDelete ||
            ProjectLenderReqStatusCode == CodeFor.ProjectLenderReqCancelDelete)
        {
            return ProgressState.Cancelled;
        }

        // Rejected has second highest priority
        if (ProjectLenderReqStatusCode == CodeFor.ProjectLenderReqReject)
        {
            return ProgressState.Rejected;
        }

        // Accepted
        if (ProjectStageStatusCode == CodeFor.ProjectStageComplete &&
            ProjectLenderReqStatusCode == CodeFor.ProjectLenderReqAccept)
        {
            return ProgressState.Accepted;
        }

        if (StageLevel == 0)
        {
            if (ProjectStageStatusCode == CodeFor.ProjectStageOnReview)
            {
                return ProgressState.Review;
            }

            if (ProjectStageStatusCode == CodeFor.ProjectStageOnProgress)
            {
                return ProjectLenderReqStatusCode switch
                {
                    CodeFor.ProjectLenderReqSubmit => ProgressState.Submit,
                    CodeFor.ProjectLenderReqRevision => ProgressState.Revision,
                    _ => ProgressState.OnProgress
                };
            }

            return ProgressState.OnProgress;
        }

        // StageLevel > 0
        if (ProjectStageStatusCode == CodeFor.ProjectStageOnReview)
        {
            return ProgressState.Review;
        }

        return ProgressState.OnProgress;
    }

    private double GetProgress()
    {
        _state = GetState();

        if (StageLevel == 0)
        {
            return _state switch
            {
                ProgressState.OnProgress => 12.5,
                ProgressState.Submit => 37.5,
                ProgressState.Revision => 37.5,
                ProgressState.Review => 62.5,
                ProgressState.Accepted => 100,
                ProgressState.Rejected => 100,
                ProgressState.Cancelled => 100,
                _ => 0
            };
        }

        return _state switch
        {
            ProgressState.OnProgress => 16.67,
            ProgressState.Submit => 0,
            ProgressState.Revision => 0,
            ProgressState.Review => 50,
            ProgressState.Accepted => 100,
            ProgressState.Rejected => 100,
            ProgressState.Cancelled => 100,
            _ => 0
        };
    }

    private Color GetOnProgressColor()
    {
        return GetState() switch
        {
            ProgressState.OnProgress => Color.Info,
            ProgressState.Submit => Color.Success,
            ProgressState.Revision => Color.Success,
            ProgressState.Review => Color.Success,
            ProgressState.Accepted => Color.Success,
            ProgressState.Rejected => Color.Error,
            ProgressState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetSubmitColor()
    {
        if (StageLevel != 0)
        {
            return Color.Default;
        }

        return GetState() switch
        {
            ProgressState.OnProgress => Color.Default,
            ProgressState.Submit => Color.Info,
            ProgressState.Revision => Color.Warning,
            ProgressState.Review => Color.Success,
            ProgressState.Accepted => Color.Success,
            ProgressState.Rejected => Color.Error,
            ProgressState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetReviewColor()
    {
        return GetState() switch
        {
            ProgressState.OnProgress => Color.Default,
            ProgressState.Submit => Color.Default,
            ProgressState.Revision => Color.Default,
            ProgressState.Review => Color.Info,
            ProgressState.Accepted => Color.Success,
            ProgressState.Rejected => Color.Error,
            ProgressState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetResultColor()
    {
        return GetState() switch
        {
            ProgressState.OnProgress => Color.Default,
            ProgressState.Submit => Color.Default,
            ProgressState.Revision => Color.Default,
            ProgressState.Review => Color.Default,
            ProgressState.Accepted => Color.Success,
            ProgressState.Rejected => Color.Error,
            ProgressState.Cancelled => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetProgressColor()
    {
        return GetState() switch
        {
            ProgressState.OnProgress => Color.Primary,
            ProgressState.Submit => Color.Primary,
            ProgressState.Revision => Color.Warning,
            ProgressState.Review => Color.Primary,
            ProgressState.Rejected => Color.Error,
            ProgressState.Accepted => Color.Success,
            ProgressState.Cancelled => Color.Error,
            _ => Color.Primary
        };
    }
}
