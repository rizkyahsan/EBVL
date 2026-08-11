using EBVL.FrontEnd.Logics.Modules.Administration.ApiCalls.GetApiCalls;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.ApiCalls.Pages;

public partial class Index
{
    private IEnumerable<ApiCallItem> _items = [];
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
            AdministrationBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ApiCallsDisplayTextFor.ApiCalls)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetApiCallsQuery();
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

    private bool FilterItems(ApiCallItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Created.ToDisplayText(DateTimeFormatFor.LongDateTime).Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ServiceName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ServiceProvider.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ServiceCategory.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.RequestMethod.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ResponseStatusCode.ToString().Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
