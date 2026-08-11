namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class Index
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
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.Examples)
        ];
    }
}
