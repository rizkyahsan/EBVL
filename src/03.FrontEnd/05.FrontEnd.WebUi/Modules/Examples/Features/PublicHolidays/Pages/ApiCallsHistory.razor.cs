namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.PublicHolidays.Pages;

public partial class ApiCallsHistory
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
            ExamplesPublicHolidaysBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(PublicHolidaysDisplayTextFor.ApiCallsHistory)
        ];
    }
}
