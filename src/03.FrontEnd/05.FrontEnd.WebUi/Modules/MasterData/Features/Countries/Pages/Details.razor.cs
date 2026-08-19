using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountry;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private CountryItem _item = default!;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = CountriesDisplayTextFor.Country;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            MasterDataCountriesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetCountryQuery
            {
                CountryId = Id
            };

            var response = await Sender.Send(query);

            _item = response.Item;
            _pageTitle = _item.Name;

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));
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

    private async Task ShowDialogDelete()
    {
        var model = new DialogDeleteModel
        {
            CountryId = _item.Id,
            CountryName = _item.Name
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {CountriesDisplayTextFor.Country}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            NavigationManager.NavigateTo(MasterDataCountriesRouteFor.Index);
        }
    }
}
