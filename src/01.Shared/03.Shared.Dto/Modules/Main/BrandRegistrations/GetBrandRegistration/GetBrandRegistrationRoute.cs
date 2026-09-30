namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistration;

public static class GetBrandRegistrationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetBrandRegistration)}";
    public const string Description = "Get Brand Registration detail";
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}";
    public static string ResourceUri(Guid registrationId)
    {
        return $"{RouteConfig.BasePath}/{registrationId}";
    }
}
