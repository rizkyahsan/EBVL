namespace EBVL.Shared.Statics.Statuses;

public static class CodeFor
{
    public const string ProjectDraft = "P0";
    public const string ProjectOnProgress = "P1";
    public const string ProjectComplete = "P2";
    public const string ProjectCancel = "P9";

    public const string ProjectLenderDraft = "PL0";
    public const string ProjectLenderOnProgress = "PL1";
    public const string ProjectLenderWin = "PL2";
    public const string ProjectLenderLose = "PL3";
    public const string ProjectLenderCancelDelete = "PL9";

    public const string ProjectStageDraft = "PS0";
    public const string ProjectStageOnProgress = "PS1";
    public const string ProjectStageOnReview = "PS2";
    public const string ProjectStageComplete = "PS3";
    public const string ProjectStageCancelDelete = "PS9";

    public const string ProjectLenderReqDraft = "PR0";
    public const string ProjectLenderReqOnProgress = "PR1";
    public const string ProjectLenderReqSubmit = "PR2";
    public const string ProjectLenderReqRevision = "PR3";
    public const string ProjectLenderReqAccept = "PR4";
    public const string ProjectLenderReqReject = "PR5";
    public const string ProjectLenderReqCancelDelete = "PR9";
}
