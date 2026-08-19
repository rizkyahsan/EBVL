using EBVL.Shared.Statics.Statuses;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Components;

public partial class ProgressBarStatusProjectStage
{
    [Parameter]
    public required string StatusCode { get; init; }
    private const string StepWidth = "25%";

    private double GetProgress()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageDraft => 12.5,      // before first step
            CodeFor.ProjectStageOnProgress => 37.5, // first step
            CodeFor.ProjectStageOnReview => 62.5,   // second step
            CodeFor.ProjectStageComplete => 100,    // finished
            CodeFor.ProjectStageCancelDelete => 0,
            _ => 0
        };
    }

    private Color GetDraftColor()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageDraft => Color.Info,
            CodeFor.ProjectStageOnProgress => Color.Success,
            CodeFor.ProjectStageOnReview => Color.Success,
            CodeFor.ProjectStageComplete => Color.Success,
            CodeFor.ProjectStageCancelDelete => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetOnProgressColor()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageOnProgress => Color.Info,
            CodeFor.ProjectStageOnReview => Color.Success,
            CodeFor.ProjectStageComplete => Color.Success,
            CodeFor.ProjectStageCancelDelete => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetReviewColor()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageOnReview => Color.Info,
            CodeFor.ProjectStageComplete => Color.Success,
            CodeFor.ProjectStageCancelDelete => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetResultColor()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageComplete => Color.Success,
            CodeFor.ProjectStageCancelDelete => Color.Error,
            _ => Color.Default
        };
    }

    private Color GetProgressColor()
    {
        return StatusCode switch
        {
            CodeFor.ProjectStageCancelDelete => Color.Error,
            CodeFor.ProjectStageComplete => Color.Success,
            _ => Color.Primary
        };
    }
}
