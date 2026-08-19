using EBVL.Shared.Statics.Statuses;

namespace EBVL.FrontEnd.WebUi.Common.Components;

public partial class ChipStatus
{
    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public string StatusCode { get; set; } = string.Empty;

    private Color GetColor()
    {
        return StatusCode switch
        {
            // Draft
            CodeFor.ProjectDraft or
            CodeFor.ProjectLenderDraft or
            CodeFor.ProjectStageDraft or
            CodeFor.ProjectLenderReqDraft
                => Color.Default,

            // Progress
            CodeFor.ProjectOnProgress or
            CodeFor.ProjectLenderOnProgress or
            CodeFor.ProjectStageOnProgress or
            CodeFor.ProjectLenderReqOnProgress
                => Color.Info,

            // Review
            CodeFor.ProjectStageOnReview
                => Color.Info,

            // Submit
            CodeFor.ProjectLenderReqSubmit
                => Color.Primary,

            // Success
            CodeFor.ProjectComplete or
            CodeFor.ProjectStageComplete or
            CodeFor.ProjectLenderWin or
            CodeFor.ProjectLenderReqAccept
                => Color.Success,

            // Rejected / Lost
            CodeFor.ProjectLenderLose or
            CodeFor.ProjectLenderReqReject
                => Color.Error,

            // Revision
            CodeFor.ProjectLenderReqRevision
                => Color.Warning,

            // Cancelled
            CodeFor.ProjectCancel or
            CodeFor.ProjectLenderCancelDelete or
            CodeFor.ProjectStageCancelDelete or
            CodeFor.ProjectLenderReqCancelDelete
                => Color.Dark,

            _ => Color.Default
        };
    }
}
