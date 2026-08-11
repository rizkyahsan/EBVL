namespace EBVL.FrontEnd.WebUi.Common.Statics;

public static class ErrorMessageFor
{
    public static string FieldIsRequired(string fieldName)
    {
        return $"{fieldName} is required.";
    }
}
