namespace EBVL.Shared.Dto.Modules.Examples.Dummies.GetDummies;

public static class GetDummiesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetDummies)}";
    public const string Description = $"{CommonDisplayTextFor.Get} Dummies";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
