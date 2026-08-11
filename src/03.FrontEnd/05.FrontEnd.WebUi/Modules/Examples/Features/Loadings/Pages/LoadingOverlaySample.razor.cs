namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Loadings.Pages;

public partial class LoadingOverlaySample
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
            ExamplesLoadingsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesLoadingsDisplayTextFor.LoadingOverlay)
        ];
    }

    private async Task ShowLoading()
    {
        _isLoading = true;

        await Task.Delay(3000);

        _isLoading = false;
    }
}
