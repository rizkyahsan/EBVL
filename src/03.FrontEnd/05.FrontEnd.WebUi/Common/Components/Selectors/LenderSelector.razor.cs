using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLenders;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

namespace EBVL.FrontEnd.WebUi.Common.Components.Selectors;

public partial class LenderSelector
{
    [Parameter]
    public bool IsNoLabel { get; set; }

    [Parameter]
    public string Label { get; set; } = LendersDisplayTextFor.Lender;

    [Parameter]
    public bool IsTenderMode { get; set; }

    [Parameter]
    public IEnumerable<Guid>? CurrentLenders { get; set; }

    private bool _isLoading = true;

    // original list
    private List<LenderItem> _allItems = [];

    // displayed list
    private List<LenderItem> _items = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            var response = await Sender.Send(new GetLendersQuery());

            _allItems = [.. response.Items];

            await ApplyFilterAsync();
        }
        catch (Exception ex)
        {
            Snackbar.AddError(ex.Message);
        }
        finally
        {
            _isLoading = false;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await ApplyFilterAsync();

        await base.OnParametersSetAsync();
    }

    private async Task ApplyFilterAsync()
    {
        if (!IsTenderMode)
        {
            _items = [.. _allItems];
            return;
        }

        var selected = CurrentLenders?.ToHashSet() ?? [];

        _items = [.. _allItems
            .Where(x => !selected.Contains(x.Id))
            .OrderBy(x => x.Name)];

        if (!IsAllowNoValue && Value == Guid.Empty && _items.Any())
        {
            Value = _items.OrderBy(x => x.Name).First().Id;

            await ValueChanged.InvokeAsync(Value);
        }
    }
}
