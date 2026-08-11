namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class Typography
{
    private const string Pangram1 = "The quick brown fox jumps over the lazy dog";
    private const string Pangram2 = "The five boxing wizards jump quickly";
    private string _text = Pangram1;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.Typography)
        ];
    }

    protected void SetToPangram1()
    {
        _text = Pangram1;
    }

    protected void SetToPangram2()
    {
        _text = Pangram2;
    }

    protected void SetToCurrentDateTime()
    {
        _text = $"What time is it? It's {DateTime.Now: dddd MMMM yyyy, HH:mm:ss}!";
    }

    protected void UsingBogus()
    {
        _text = FakerFor.English.Lorem.Sentence();
    }
}
