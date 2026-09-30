namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.AssignBrandRegistration;

public static class AssignBrandRegistrationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AssignBrandRegistration)}";
    public const string Description = "Assign a Brand Registration workflow";
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}/assignments";
}
