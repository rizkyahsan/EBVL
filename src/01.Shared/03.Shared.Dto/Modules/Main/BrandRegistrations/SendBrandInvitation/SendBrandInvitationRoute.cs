namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.SendBrandInvitation;

public static class SendBrandInvitationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(SendBrandInvitation)}";
    public const string Description = "Send a Brand Registration invitation";
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}/invitations";
}
