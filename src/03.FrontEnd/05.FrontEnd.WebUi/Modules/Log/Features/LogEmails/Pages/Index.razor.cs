using EBVL.FrontEnd.Logics.Modules.Log.LogEmails.GetLogEmail;
using EBVL.FrontEnd.Logics.Modules.Log.LogEmails.GetLogEmails;
using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;
using EBVL.Shared.Statics.Common;

namespace EBVL.FrontEnd.WebUi.Modules.Log.Features.LogEmails.Pages;

public partial class Index
{
    private IEnumerable<LogEmailItem> _items = [];
    private static DateTime TodayWib => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneFor.WibTimeZone).Date;
    private DateRange _dateRange = new(TodayWib, TodayWib);
    private DateTime StartDate => _dateRange.Start ?? TodayWib;
    private DateTime EndDate => _dateRange.End ?? TodayWib;
    private string? _searchKeyword;
    private bool _isDrawerDetailsOpen;
    private string _selectedPreset = "Today";
    private Shared.Dto.Modules.Log.LogEmails.GetLogEmail.LogEmailItem? _selectedLogEmail;

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
            LogBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(LogEmailsDisplayTextFor.LogEmails)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetLogEmailsQuery()
            {
                StartDatetime = StartDate,
                EndDatetime = EndDate.AddDays(1).AddTicks(-1)
            };
            var response = await Sender.Send(query);

            _items = response.Items;
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnDateRangeChanged(DateRange range)
    {
        _dateRange = range;

        if (range.Start is not null &&
            range.End is not null)
        {
            await LoadItems();
        }
    }

    private async Task SetToday()
    {
        _dateRange = new DateRange(TodayWib, TodayWib);
        _selectedPreset = "Today";

        await LoadItems();
    }

    private async Task SetYesterday()
    {
        var yesterday = TodayWib.AddDays(-1);

        _dateRange = new DateRange(yesterday, yesterday);
        _selectedPreset = "Yesterday";

        await LoadItems();
    }

    private async Task SetLast7Days()
    {
        _dateRange = new DateRange(TodayWib.AddDays(-6), TodayWib);
        _selectedPreset = "Last7Days";

        await LoadItems();
    }

    private async Task SetLast30Days()
    {
        _dateRange = new DateRange(TodayWib.AddDays(-29), TodayWib);
        _selectedPreset = "Last30Days";

        await LoadItems();
    }

    private bool FilterItems(LogEmailItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        return (item.Module?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
            || (item.Action?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private async Task ShowDrawerDetails(Guid id)
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetLogEmailQuery
            {
                Id = id
            };

            var response = await Sender.Send(query);

            _selectedLogEmail = response.Item;
            _isDrawerDetailsOpen = true;
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }
    }
}
