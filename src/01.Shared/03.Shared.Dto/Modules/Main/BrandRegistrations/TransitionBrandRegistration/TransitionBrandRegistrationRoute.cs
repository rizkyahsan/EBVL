namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.TransitionBrandRegistration;

public static class TransitionBrandRegistrationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(TransitionBrandRegistration)}";
    public const string Description = "Transition a Brand Registration workflow";
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}/transitions";
}
