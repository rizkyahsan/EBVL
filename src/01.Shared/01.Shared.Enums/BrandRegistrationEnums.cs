namespace EBVL.Shared.Enums;

public enum BrandRegistrationStatus
{
    Draft,
    Submitted,
    NeedAssignmentBySeniorManagerMsai,
    RequestApprovalByAnalystReliability,
    ReviewByAnalystReliability,
    NeedRevisionByVendor,
    RequestApprovalBySeniorManagerMsai,
    ReviewBySeniorManagerMsai,
    NeedAssignmentByChiefSection,
    RequestEvaluationBySpecialistSection,
    EvaluationBySpecialistSection,
    EvaluationObjectionByVendor,
    RequestEvaluationApprovalBySeniorManagerMsai,
    InviteToPresent,
    Presentation,
    RequestPresentationEvaluationBySpecialist,
    PresentationEvaluationBySpecialist,
    RequestPresentationEvaluationApprovalByChiefSection,
    RequestApprovalByVpSection,
    ReviewByVpSection,
    RequestApprovalByVpReliability,
    ReviewByVpReliability,
    RequestClosingByAdminMsai,
    ClosingByAdminMsai,
    Approved,
    Rejected
}

public enum BrandRegistrationAction
{
    Submit,
    StartAssignment,
    Assign,
    StartReview,
    Approve,
    NeedRevision,
    Resubmit,
    RaiseObjection,
    ResolveObjection,
    SendInvitation,
    ConfirmInvitation,
    RejectInvitation,
    StartClosing,
    Close,
    Reject
}

public enum BrandInvitationResponse
{
    Confirmed,
    Rejected
}
