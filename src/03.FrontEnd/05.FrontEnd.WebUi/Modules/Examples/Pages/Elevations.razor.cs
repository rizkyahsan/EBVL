namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class Elevations
{
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
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.Elevations)
        ];
    }
}
