namespace EBVL.Shared.Dto.Modules.Examples.Dummies.PostDummy;

public static class PostDummyRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(PostDummy)}";
    public const string Description = "Post Dummy";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
