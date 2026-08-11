namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class About
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
            CommonBreadcrumbFor.Active(MainDisplayTextFor.About)
        ];
    }
}
