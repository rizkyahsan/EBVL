using EBVL.FrontEnd.Logics.Modules.Administration.Audits.GetAudits;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Audits.Pages;

public partial class Index
{
    private MudTable<AuditItem> _table = default!;
    private string? _searchKeyword;
    private readonly SearchFilterModel _searchFilterModel = new();

    protected override void OnInitialized()
    {
        _pageTitle = AuditsDisplayTextFor.Audits;

        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            AdministrationBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task<TableData<AuditItem>> ReloadTable(TableState state, CancellationToken token)
    {
        var tableData = new TableData<AuditItem>();

        try
        {
            _isLoading = true;

            ClearException();

            var query = state.ToPaginatedListRequest<GetAuditsQuery>(_searchKeyword);
            query.From = _searchFilterModel.From;
            query.To = _searchFilterModel.To;

            var response = await Sender.Send(query, token);

            tableData = response.ToTableData();
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }

        return tableData;
    }

    private async Task OnSearch(string keyword)
    {
        _searchKeyword = keyword.Trim();

        await _table.ReloadServerData();
    }

    private async Task Search()
    {
        await _table.ReloadServerData();
    }
}

public sealed record SearchFilterModel
{
    private static readonly DateTime _fromDateDefaultValue = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Local).AddDays(-7);
    private static readonly DateTime _toDateDefaultValue = DateTime.SpecifyKind(DateTime.Now.Date, DateTimeKind.Local);

    public DateTime? FromDate { get; set; } = _fromDateDefaultValue;
    public TimeSpan? FromTime { get; set; } = new TimeSpan(0, 0, 0);
    public DateTime? ToDate { get; set; } = _toDateDefaultValue;
    public TimeSpan? ToTime { get; set; } = new TimeSpan(23, 59, 59);

    public DateTimeOffset From
    {
        get
        {
            var from = FromDate.HasValue
                ? TimeZoneInfo.ConvertTime(new DateTime(FromDate.Value.Ticks, DateTimeKind.Local), TimeZoneInfo.Local)
                : TimeZoneInfo.ConvertTime(new DateTime(_fromDateDefaultValue.Ticks, DateTimeKind.Local), TimeZoneInfo.Local);

            if (FromTime.HasValue)
            {
                from = from.AddTicks(FromTime.Value.Ticks);
            }

            return from;
        }
    }

    public DateTimeOffset To
    {
        get
        {
            var to = ToDate.HasValue
                ? TimeZoneInfo.ConvertTime(new DateTime(ToDate.Value.Ticks, DateTimeKind.Local), TimeZoneInfo.Local)
                : TimeZoneInfo.ConvertTime(new DateTime(_toDateDefaultValue.Ticks, DateTimeKind.Local), TimeZoneInfo.Local);

            if (ToTime.HasValue)
            {
                to = to.AddTicks(ToTime.Value.Ticks);
            }

            return to;
        }
    }
}
