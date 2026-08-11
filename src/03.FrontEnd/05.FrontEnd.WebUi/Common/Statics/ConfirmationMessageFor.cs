namespace EBVL.FrontEnd.WebUi.Common.Statics;

public static class ConfirmationMessageFor
{
    public static string Delete(string entityType, string entityName)
    {
        return $"Are you sure you want to delete {entityType} {entityName}?";
    }

    public static string Delete(string entityType)
    {
        return $"Are you sure you want to delete this {entityType}?";
    }
}
