using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.WebUi.Common.Components.Selectors;

public partial class CountrySelector
{
    [Parameter]
    public bool IsNoLabel { get; set; }

    [Parameter]
    public string Label { get; set; } = CountriesDisplayTextFor.Country;

    [Parameter]
    public EventCallback<CountryItem?> SelectedCountryChanged { get; set; }

    private List<CountryItem> _items = [];
    private bool _isLoading;
    private Guid? _lastValue;
    private CountryItem? SelectedCountry => _items.FirstOrDefault(x => x.Id.Equals(Value));

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;

        try
        {
            var response = await Sender.Send(new GetCountriesQuery());

            _items = [.. response.Items];

            if (!IsAllowNoValue && Value == Guid.Empty && _items.Count > 0)
            {
                await OnCountryChanged(_items.OrderBy(x => x.Name).First());
            }
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
        await base.OnParametersSetAsync();

        if (_isLoading)
        {
            return;
        }

        if (_lastValue == Value)
        {
            return;
        }

        _lastValue = Value;

        var country = SelectedCountry;

        await SelectedCountryChanged.InvokeAsync(country);
    }

    private async Task<IEnumerable<CountryItem>> SearchCountries(string value, CancellationToken token)
    {
        await Task.Delay(300, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return _items.OrderBy(x => x.Name);
        }

        return _items.Where(x =>
                x.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                x.PhoneCode.Contains(value, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name);
    }

    private async Task OnCountryChanged(CountryItem? country)
    {
        await OnValueChanged(country?.Id ?? Guid.Empty);

        await SelectedCountryChanged.InvokeAsync(country);
    }
}
