namespace EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

public static class GetQuestionnairesRoute { public const string Pattern = RouteConfig.BasePath; }
public static class AddQuestionnaireRoute { public const string Pattern = RouteConfig.BasePath; }
public static class GetQuestionnaireRoute
{
    public const string Pattern = RouteConfig.BasePath + "/{questionnaireId:guid}"; public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
public static class UpdateQuestionnaireRoute
{
    public const string Pattern = GetQuestionnaireRoute.Pattern; public static string ResourceUri(Guid id)
    {
        return GetQuestionnaireRoute.ResourceUri(id);
    }
}
public static class GetQuestionnaireSectionsRoute
{
    public const string Pattern = GetQuestionnaireRoute.Pattern + "/sections"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRoute.ResourceUri(id)}/sections";
    }
}
public static class AddQuestionnaireSectionRoute
{
    public const string Pattern = GetQuestionnaireSectionsRoute.Pattern; public static string ResourceUri(Guid id)
    {
        return GetQuestionnaireSectionsRoute.ResourceUri(id);
    }
}
public static class UpdateQuestionnaireSectionRoute
{
    public const string Pattern = GetQuestionnaireSectionsRoute.Pattern + "/{sectionId:guid}"; public static string ResourceUri(Guid id, Guid sectionId)
    {
        return $"{GetQuestionnaireSectionsRoute.ResourceUri(id)}/{sectionId}";
    }
}
public static class DeleteQuestionnaireSectionRoute
{
    public const string Pattern = UpdateQuestionnaireSectionRoute.Pattern; public static string ResourceUri(Guid id, Guid sectionId)
    {
        return UpdateQuestionnaireSectionRoute.ResourceUri(id, sectionId);
    }
}
public static class GetQuestionnaireQuestionsRoute
{
    public const string Pattern = UpdateQuestionnaireSectionRoute.Pattern + "/questions"; public static string ResourceUri(Guid id, Guid sectionId)
    {
        return $"{UpdateQuestionnaireSectionRoute.ResourceUri(id, sectionId)}/questions";
    }
}
public static class AddQuestionnaireQuestionRoute
{
    public const string Pattern = GetQuestionnaireQuestionsRoute.Pattern; public static string ResourceUri(Guid id, Guid sectionId)
    {
        return GetQuestionnaireQuestionsRoute.ResourceUri(id, sectionId);
    }
}
public static class GetQuestionnaireQuestionRoute
{
    public const string Pattern = GetQuestionnaireQuestionsRoute.Pattern + "/{questionId:guid}"; public static string ResourceUri(Guid id, Guid sectionId, Guid questionId)
    {
        return $"{GetQuestionnaireQuestionsRoute.ResourceUri(id, sectionId)}/{questionId}";
    }
}
public static class UpdateQuestionnaireQuestionRoute
{
    public const string Pattern = GetQuestionnaireQuestionRoute.Pattern; public static string ResourceUri(Guid id, Guid sectionId, Guid questionId)
    {
        return GetQuestionnaireQuestionRoute.ResourceUri(id, sectionId, questionId);
    }
}
public static class DeleteQuestionnaireQuestionRoute
{
    public const string Pattern = GetQuestionnaireQuestionRoute.Pattern; public static string ResourceUri(Guid id, Guid sectionId, Guid questionId)
    {
        return GetQuestionnaireQuestionRoute.ResourceUri(id, sectionId, questionId);
    }
}
public static class CreateQuestionnaireDraftRoute
{
    public const string Pattern = GetQuestionnaireRoute.Pattern + "/draft"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRoute.ResourceUri(id)}/draft";
    }
}
public static class PublishQuestionnaireRoute
{
    public const string Pattern = GetQuestionnaireRoute.Pattern + "/publish"; public static string ResourceUri(Guid id)
    {
        return $"{GetQuestionnaireRoute.ResourceUri(id)}/publish";
    }
}
public static class GetQuestionnaireVersionHistoryRoute
{
    public const string Pattern = RouteConfig.BasePath + "/series/{seriesId:guid}/versions"; public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/series/{id}/versions";
    }
}
