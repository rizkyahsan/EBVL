using EBVL.FrontEnd.Logics.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;
using Pertamina.Common.Dto.Enums;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Pages;

public partial class Index
{
    private static readonly TimeZoneInfo _wibTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
    private MudTable<RequestRegistrationItem> _table = default!;
    private IReadOnlyList<RequestRegistrationItem> _currentRows = [];
    private RequestRegistrationCategory _category = RequestRegistrationCategory.Process;
    private string? _searchKeyword;
    private int _page;
    private int _pageSize = 10;
    private int _processCount;
    private int _approveCount;
    private int _rejectCount;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            VendorRequestRegistrationsBreadcrumbFor.Vendor,
            CommonBreadcrumbFor.Active(VendorRequestRegistrationsDisplayTextFor.RequestRegistration)
        ];
    }

    private async Task<TableData<RequestRegistrationItem>> ReloadTable(TableState state, CancellationToken cancellationToken)
    {
        _page = state.Page;
        _pageSize = state.PageSize;

        try
        {
            _isLoading = true;
            ClearException();
            var response = await Sender.Send(new GetRequestRegistrationsQuery
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SearchText = _searchKeyword,
                Category = _category,
                SortField = state.SortLabel,
                SortOrder = state.SortDirection == SortDirection.Ascending
                    ? SortOrder.Ascending
                    : state.SortDirection == SortDirection.Descending ? SortOrder.Descending : null
            }, cancellationToken);

            _processCount = response.ProcessCount;
            _approveCount = response.ApproveCount;
            _rejectCount = response.RejectCount;
            _currentRows = response.Items.ToList();
            await InvokeAsync(StateHasChanged);
            return response.ToTableData();
        }
        catch (Exception exception)
        {
            _exception = exception;
            return new TableData<RequestRegistrationItem>();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SelectCategory(RequestRegistrationCategory category)
    {
        if (_category == category)
        {
            return;
        }

        _category = category;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(string value)
    {
        _searchKeyword = value.Trim();
        await _table.ReloadServerData();
    }

    private int Count(RequestRegistrationCategory category)
    {
        return category switch
        {
            RequestRegistrationCategory.Process => _processCount,
            RequestRegistrationCategory.Approve => _approveCount,
            RequestRegistrationCategory.Reject => _rejectCount,
            _ => 0
        };
    }

    private int Number(RequestRegistrationItem item)
    {
        return (_page * _pageSize) + _currentRows.ToList().IndexOf(item) + 1;
    }

    private static Color CategoryColor(RequestRegistrationCategory category)
    {
        return category switch
        {
            RequestRegistrationCategory.Process => Color.Warning,
            RequestRegistrationCategory.Approve => Color.Success,
            RequestRegistrationCategory.Reject => Color.Error,
            _ => Color.Default
        };
    }

    private static string FormatWib(DateTimeOffset value)
    {
        return $"{TimeZoneInfo.ConvertTime(value, _wibTimeZone):dd MMM yyyy HH:mm} WIB";
    }
}
