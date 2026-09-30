using EBVL.BackEnd.Logics.Modules.Main.BrandRegistrations;
using EBVL.Shared.Enums;
using EBVL.Shared.Statics;
using FluentValidation;
using Xunit;

namespace EBVL.BackEnd.Logics.Tests;

public sealed class BrandRegistrationWorkflowTests
{
    [Theory]
    [InlineData(BrandRegistrationStatus.Draft, BrandRegistrationAction.Submit, BrandRegistrationStatus.Submitted, RoleNameFor.Vendor)]
    [InlineData(BrandRegistrationStatus.NeedAssignmentBySeniorManagerMsai, BrandRegistrationAction.Assign, BrandRegistrationStatus.RequestApprovalByAnalystReliability, RoleNameFor.SeniorManagerMsai)]
    [InlineData(BrandRegistrationStatus.ReviewByAnalystReliability, BrandRegistrationAction.NeedRevision, BrandRegistrationStatus.NeedRevisionByVendor, RoleNameFor.AnalisReliability)]
    [InlineData(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.ConfirmInvitation, BrandRegistrationStatus.Presentation, RoleNameFor.Vendor)]
    [InlineData(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.RejectInvitation, BrandRegistrationStatus.Rejected, RoleNameFor.Vendor)]
    [InlineData(BrandRegistrationStatus.ClosingByAdminMsai, BrandRegistrationAction.Close, BrandRegistrationStatus.Approved, RoleNameFor.AdminMsai)]
    public void ReturnsCanonicalTransition(BrandRegistrationStatus from, BrandRegistrationAction action, BrandRegistrationStatus to, string role)
    {
        var rule = BrandRegistrationWorkflow.GetRule(from, action);

        Assert.Equal(to, rule.To);
        Assert.Equal(role, rule.Role);
    }

    [Theory]
    [InlineData(BrandRegistrationStatus.Approved)]
    [InlineData(BrandRegistrationStatus.Rejected)]
    public void TerminalStatusRejectsFurtherTransition(BrandRegistrationStatus status)
    {
        _ = Assert.Throws<ValidationException>(() => BrandRegistrationWorkflow.GetRule(status, BrandRegistrationAction.Approve));
    }

    [Fact]
    public void InvitationRejectionCompletesWorkflow()
    {
        var rule = BrandRegistrationWorkflow.GetRule(BrandRegistrationStatus.InviteToPresent, BrandRegistrationAction.RejectInvitation);

        Assert.True(rule.Complete);
        Assert.Null(rule.NextRole);
    }
}
