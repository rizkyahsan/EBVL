namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;

public static class CreateBrandRegistrationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CreateBrandRegistration)}";
    public const string Description = "Create a Brand Registration draft or submission";
    public const string Pattern = RouteConfig.BasePath;
}
