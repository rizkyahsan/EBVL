using EBVL.Shared.Statics;

namespace EBVL.BackEnd.Logics.Modules.Main.BrandRegistrations;

public sealed record BrandWorkflowRule(BrandRegistrationStatus To, string Role, bool RequiresAssignment, string? NextRole = null, bool Complete = false);

public static class BrandRegistrationWorkflow
{
    private static readonly IReadOnlyDictionary<(BrandRegistrationStatus, BrandRegistrationAction), BrandWorkflowRule> _rules =
        new Dictionary<(BrandRegistrationStatus, BrandRegistrationAction), BrandWorkflowRule>
        {
            [(BrandRegistrationStatus.Draft, BrandRegistrationAction.Submit)] = new(BrandRegistrationStatus.Submitted, RoleNameFor.Vendor, false, RoleNameFor.SeniorManagerMsai),
            [(BrandRegistrationStatus.NeedAssignmentBySeniorManagerMsai, BrandRegistrationAction.Assign)] = new(BrandRegistrationStatus.RequestApprovalByAnalystReliability, RoleNameFor.SeniorManagerMsai, true, RoleNameFor.AnalisReliability),
            [(BrandRegistrationStatus.RequestApprovalByAnalystReliability, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.ReviewByAnalystReliability, RoleNameFor.AnalisReliability, true),
            [(BrandRegistrationStatus.ReviewByAnalystReliability, BrandRegistrationAction.NeedRevision)] = new(BrandRegistrationStatus.NeedRevisionByVendor, RoleNameFor.AnalisReliability, true, RoleNameFor.Vendor),
            [(BrandRegistrationStatus.ReviewByAnalystReliability, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestApprovalBySeniorManagerMsai, RoleNameFor.AnalisReliability, true, RoleNameFor.SeniorManagerMsai),
            [(BrandRegistrationStatus.NeedRevisionByVendor, BrandRegistrationAction.Resubmit)] = new(BrandRegistrationStatus.RequestApprovalByAnalystReliability, RoleNameFor.Vendor, true, RoleNameFor.AnalisReliability),
            [(BrandRegistrationStatus.RequestApprovalBySeniorManagerMsai, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.ReviewBySeniorManagerMsai, RoleNameFor.SeniorManagerMsai, true),
            [(BrandRegistrationStatus.ReviewBySeniorManagerMsai, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.NeedAssignmentByChiefSection, RoleNameFor.SeniorManagerMsai, true, RoleNameFor.ChiefSection),
            [(BrandRegistrationStatus.NeedAssignmentByChiefSection, BrandRegistrationAction.Assign)] = new(BrandRegistrationStatus.RequestEvaluationBySpecialistSection, RoleNameFor.ChiefSection, true, RoleNameFor.SpecialistSection),
            [(BrandRegistrationStatus.RequestEvaluationBySpecialistSection, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.EvaluationBySpecialistSection, RoleNameFor.SpecialistSection, true),
            [(BrandRegistrationStatus.EvaluationBySpecialistSection, BrandRegistrationAction.RaiseObjection)] = new(BrandRegistrationStatus.EvaluationObjectionByVendor, RoleNameFor.SpecialistSection, true, RoleNameFor.Vendor),
            [(BrandRegistrationStatus.EvaluationBySpecialistSection, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestEvaluationApprovalBySeniorManagerMsai, RoleNameFor.SpecialistSection, true, RoleNameFor.SeniorManagerMsai),
            [(BrandRegistrationStatus.EvaluationObjectionByVendor, BrandRegistrationAction.ResolveObjection)] = new(BrandRegistrationStatus.RequestEvaluationBySpecialistSection, RoleNameFor.Vendor, true, RoleNameFor.SpecialistSection),
            [(BrandRegistrationStatus.RequestEvaluationApprovalBySeniorManagerMsai, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.InviteToPresent, RoleNameFor.SeniorManagerMsai, true, RoleNameFor.AdminMsai),
            [(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.SendInvitation)] = new(BrandRegistrationStatus.InviteToPresent, RoleNameFor.AdminMsai, true, RoleNameFor.Vendor),
            [(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.ConfirmInvitation)] = new(BrandRegistrationStatus.Presentation, RoleNameFor.Vendor, true, RoleNameFor.AdminMsai),
            [(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.RejectInvitation)] = new(BrandRegistrationStatus.Rejected, RoleNameFor.Vendor, true, null, true),
            [(BrandRegistrationStatus.Presentation, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestPresentationEvaluationBySpecialist, RoleNameFor.AdminMsai, true, RoleNameFor.SpecialistSection),
            [(BrandRegistrationStatus.RequestPresentationEvaluationBySpecialist, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.PresentationEvaluationBySpecialist, RoleNameFor.SpecialistSection, true),
            [(BrandRegistrationStatus.PresentationEvaluationBySpecialist, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestPresentationEvaluationApprovalByChiefSection, RoleNameFor.SpecialistSection, true, RoleNameFor.ChiefSection),
            [(BrandRegistrationStatus.RequestPresentationEvaluationApprovalByChiefSection, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestApprovalByVpSection, RoleNameFor.ChiefSection, true, RoleNameFor.VpSection),
            [(BrandRegistrationStatus.RequestApprovalByVpSection, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.ReviewByVpSection, RoleNameFor.VpSection, true),
            [(BrandRegistrationStatus.ReviewByVpSection, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestApprovalByVpReliability, RoleNameFor.VpSection, true, RoleNameFor.VpReliability),
            [(BrandRegistrationStatus.RequestApprovalByVpReliability, BrandRegistrationAction.StartReview)] = new(BrandRegistrationStatus.ReviewByVpReliability, RoleNameFor.VpReliability, true),
            [(BrandRegistrationStatus.ReviewByVpReliability, BrandRegistrationAction.Approve)] = new(BrandRegistrationStatus.RequestClosingByAdminMsai, RoleNameFor.VpReliability, true, RoleNameFor.AdminMsai),
            [(BrandRegistrationStatus.RequestClosingByAdminMsai, BrandRegistrationAction.StartClosing)] = new(BrandRegistrationStatus.ClosingByAdminMsai, RoleNameFor.AdminMsai, true),
            [(BrandRegistrationStatus.ClosingByAdminMsai, BrandRegistrationAction.Close)] = new(BrandRegistrationStatus.Approved, RoleNameFor.AdminMsai, true, null, true)
        };

    public static BrandWorkflowRule GetRule(BrandRegistrationStatus status, BrandRegistrationAction action)
    {
        return _rules.TryGetValue((status, action), out var rule)
            ? rule
            : throw new ValidationException($"Action '{action}' is not allowed from status '{status}'.");
    }
}
