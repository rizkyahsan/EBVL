using EBVL.FrontEnd.Logics.Modules.Examples.PublicHolidays.GetPublicHolidays;
using EBVL.FrontEnd.WebUi.Modules.Examples.Features.PublicHolidays.Components;
using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.PublicHolidays.Pages;

public partial class Index
{
    private IEnumerable<PublicHolidayItem> _items = [];
    private string? _searchKeyword;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadItems();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(PublicHolidaysDisplayTextFor.PublicHolidays)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetPublicHolidaysQuery();
            var response = await Sender.Send(query);

            _items = response.Items;
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private bool FilterItems(PublicHolidayItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Date.ToDisplayText(DateFormatFor.LongDate).Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.CountryCode.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Name.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.LocalName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task ShowDialogLoadPublicHolidays()
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraLarge
        };

        var dialog = await DialogService.ShowAsync<DialogLoadPublicHolidays>($"{CommonDisplayTextFor.Load} {PublicHolidaysDisplayTextFor.PublicHolidays}", options);
        _ = await dialog.Result;
        await LoadItems();
    }
}
