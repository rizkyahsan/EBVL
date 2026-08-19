namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Dummies.Statics;

public static class DisplayTextFor
{
    public const string Dummies = nameof(Dummies);
    public const string Dummy = nameof(Dummy);
    public const string GetDummies = $"{CommonDisplayTextFor.Get} {Dummies}";
    public const string PostDummy = $"{CommonDisplayTextFor.Post} {Dummy}";
}
