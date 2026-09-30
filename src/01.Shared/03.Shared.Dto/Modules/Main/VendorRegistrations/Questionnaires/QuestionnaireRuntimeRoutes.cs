namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

public static class CheckSapAvailabilityRoute { public const string Pattern = RouteConfig.BasePath + "/sap-availability"; }
public static class StartQuestionnaireRoute { public const string Pattern = RouteConfig.BasePath + "/start"; }
public static class GetQuestionnaireRuntimeRoute
{
    public const string Pattern = RouteConfig.BasePath + "/{registrationId:guid}"; public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
public static class SaveQuestionnaireAnswersRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/answers"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/answers";
    }
}
public static class UpdateVendorRegistrationProfileRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/profile"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/profile";
    }
}
public static class SubmitQuestionnaireRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/submit"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/submit";
    }
}
public static class UploadQuestionnaireFileRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/questions/{questionId:guid}/files"; public static string ResourceUri(Guid id, Guid questionId)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/questions/{questionId}/files";
    }
}
public static class DeleteQuestionnaireFileRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/files/{fileId:guid}"; public static string ResourceUri(Guid id, Guid fileId)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/files/{fileId}";
    }
}
public static class DownloadQuestionnaireFileRoute
{
    public const string Pattern = DeleteQuestionnaireFileRoute.Pattern + "/download"; public static string ResourceUri(Guid id, Guid fileId)
    {
        return $"{DeleteQuestionnaireFileRoute.ResourceUri(id, fileId)}/download";
    }
}
public static class UploadVendorRegistrationDocumentRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/documents/{definitionKey}"; public static string ResourceUri(Guid id, string key)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/documents/{Uri.EscapeDataString(key)}";
    }
}
public static class DeleteVendorRegistrationDocumentRoute
{
    public const string Pattern = GetQuestionnaireRuntimeRoute.Pattern + "/documents/files/{documentId:guid}"; public static string ResourceUri(Guid id, Guid documentId)
    {
        return $"{GetQuestionnaireRuntimeRoute.ResourceUri(id)}/documents/files/{documentId}";
    }
}
public static class DownloadVendorRegistrationDocumentRoute
{
    public const string Pattern = DeleteVendorRegistrationDocumentRoute.Pattern + "/download"; public static string ResourceUri(Guid id, Guid documentId)
    {
        return $"{DeleteVendorRegistrationDocumentRoute.ResourceUri(id, documentId)}/download";
    }
}
