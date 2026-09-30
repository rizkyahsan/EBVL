namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.RespondBrandInvitation;

public static class RespondBrandInvitationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(RespondBrandInvitation)}";
    public const string Description = "Respond to a Brand Registration invitation";
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}/invitations/{invitationId:guid}/responses";
}
