namespace EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

public static class GetApiCallRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetApiCall)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ApiCallsDisplayTextFor.ApiCall}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{apiCallId:guid}}";

    public static string ResourceUri(Guid apiCallId)
    {
        return $"{RouteConfig.BasePath}/{apiCallId}";
    }
}
