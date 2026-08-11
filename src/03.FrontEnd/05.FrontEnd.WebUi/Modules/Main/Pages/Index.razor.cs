namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Index
{
    protected override void OnInitialized()
    {
        _pageTitle = $"Welcome to {AppConfigFrontEndOptions.Value.AppFullName}";

        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
    }
}
