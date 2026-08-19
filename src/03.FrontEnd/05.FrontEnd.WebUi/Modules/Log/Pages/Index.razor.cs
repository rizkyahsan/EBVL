namespace EBVL.FrontEnd.WebUi.Modules.Log.Pages;

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
            CommonBreadcrumbFor.Active(LogDisplayTextFor.Log)
        ];
    }
}
