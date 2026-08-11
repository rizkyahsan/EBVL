namespace EBVL.FrontEnd.WebUi.Common.Statics;

public static class SuccessMessageFor
{
    public static string Action(string entityType, string entityName, string actionName)
    {
        return $"{entityType} {entityName} has been successfully {actionName.ToLower()}.";
    }

    public static string Action(string entityName, string actionName)
    {
        return $"{entityName} has been successfully {actionName.ToLower()}.";
    }

    public static string ActionTo(string entityName, string actionName, string destinationEntityName)
    {
        return $"{entityName} has been successfully {actionName.ToLower()} to {destinationEntityName}.";
    }

    public static string Added(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Added);
    }

    public static string Added(string entityName)
    {
        return Action(entityName, CommonDisplayTextFor.Added);
    }

    public static string AddedTo(string entityName, string destinationEntityName)
    {
        return ActionTo(entityName, CommonDisplayTextFor.Added, destinationEntityName);
    }

    public static string Created(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Created);
    }

    public static string Updated(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Updated);
    }

    public static string Updated(string entityName)
    {
        return Action(entityName, CommonDisplayTextFor.Updated);
    }

    public static string Removed(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Removed);
    }

    public static string Removed(string entityName)
    {
        return Action(entityName, CommonDisplayTextFor.Removed);
    }

    public static string Deleted(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Deleted);
    }

    public static string Deleted(string entityType)
    {
        return Action(entityType, CommonDisplayTextFor.Deleted);
    }

    public static string Activated(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Activated);
    }

    public static string Deactivated(string entityType, string entityName)
    {
        return Action(entityType, entityName, CommonDisplayTextFor.Deactivated);
    }
}
