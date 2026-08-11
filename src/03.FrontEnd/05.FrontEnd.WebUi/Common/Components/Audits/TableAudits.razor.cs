using EBVL.Shared.Dto.Common.Audits;

namespace EBVL.FrontEnd.WebUi.Common.Components.Audits;

public partial class TableAudits
{
    [Parameter]
    public required IEnumerable<AuditItemBase> Audits { get; init; }

    private IEnumerable<AuditItemBase> _audits = [];

    protected override void OnParametersSet()
    {
        _audits = Audits.OrderByDescending(x => x.Created);
    }

    private string? _searchKeyword;

    private bool FilterItems(AuditItemBase item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Created.ToDisplayText(DateTimeFormatFor.LongDateTime).Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.CreatedBy.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ActionType.GetDisplayText().Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.ActionName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task ShowDialogAuditProperties(AuditItemBase audit)
    {
        var parameters = new DialogParameters<DialogAuditProperties>
        {
            { x => x.Audit, audit }
        };

        _ = await DialogService.ShowAsync<DialogAuditProperties>(AuditsDisplayTextFor.UpdatedProperties, parameters);
    }
}
