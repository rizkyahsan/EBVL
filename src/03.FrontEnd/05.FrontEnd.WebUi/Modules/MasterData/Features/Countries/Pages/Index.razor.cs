using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountries;
using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountry;
using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.UpdateCountry;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Pages;

public partial class Index
{
    private IEnumerable<CountryItem> _items = [];
    private string? _searchKeyword;
    private bool _isDrawerAddOpen;
    private bool _isDrawerDetailsOpen;
    private bool _isDrawerEditOpen;
    private Shared.Dto.Modules.MasterData.Countries.GetCountry.CountryItem? _selectedCountry;
    private UpdateCountryCommand? _model;

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
            MasterDataBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(CountriesDisplayTextFor.Countries)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetCountriesQuery();
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

    private bool FilterItems(CountryItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Name.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Code.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private void ShowDrawerAdd()
    {
        _isDrawerAddOpen = true;
    }

    private async Task ShowDrawerDetails(Guid countryId)
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetCountryQuery
            {
                CountryId = countryId
            };

            var response = await Sender.Send(query);

            _selectedCountry = response.Item;
            _isDrawerDetailsOpen = true;
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

    private void ShowDrawerEdit(CountryItem country)
    {
        _model = new UpdateCountryCommand
        {
            CountryId = country.Id,
            Name = country.Name,
            Code = country.Code
        };

        _isDrawerEditOpen = true;
    }

    private async Task ShowDialogDelete(CountryItem item)
    {
        var model = new DialogDeleteModel
        {
            CountryId = item.Id,
            CountryName = item.Name
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {CountriesDisplayTextFor.Country}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItems();
        }
    }
}
