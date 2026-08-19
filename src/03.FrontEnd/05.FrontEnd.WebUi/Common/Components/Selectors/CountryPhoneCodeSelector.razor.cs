using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.WebUi.Common.Components.Selectors;

public partial class CountryPhoneCodeSelector
{
    [Parameter]
    public bool IsNoLabel { get; set; }

    [Parameter]
    public string Label { get; set; } = "Phone Code";

    private List<CountryItem> _items = [];
    private bool _isLoading;

    private CountryItem? SelectedCountry =>
        string.IsNullOrWhiteSpace(Value) ? null : _items.FirstOrDefault(x => x.PhoneCode == Value);

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;

        try
        {
            var response = await Sender.Send(new GetCountriesQuery());

            _items = [.. response.Items];

            if (!IsAllowNoValue && string.IsNullOrWhiteSpace(Value) && _items.Count > 0)
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

    private async Task<IEnumerable<CountryItem>> SearchCountries(
        string value,
        CancellationToken token)
    {
        await Task.Delay(300, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return _items.OrderBy(x => x.Name);
        }

        return _items
            .Where(x =>
                x.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                x.PhoneCode.Contains(value, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name);
    }

    private async Task OnCountryChanged(CountryItem? country)
    {
        await OnValueChanged(country?.PhoneCode ?? string.Empty);
    }
}
