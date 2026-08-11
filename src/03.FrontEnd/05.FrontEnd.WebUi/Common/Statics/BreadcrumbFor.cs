namespace EBVL.FrontEnd.WebUi.Common.Statics;

public static class BreadcrumbFor
{
    public static BreadcrumbItem Active(string text)
    {
        return new(text, null, true);
    }
}
