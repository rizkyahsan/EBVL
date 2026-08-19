using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLenders;
using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLender;
using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.UpdateLender;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Pages;

public partial class Index
{
    private IEnumerable<LenderItem> _items = [];
    private string? _searchKeyword;
    private bool _isDrawerAddOpen;
    private bool _isDrawerDetailsOpen;
    private bool _isDrawerEditOpen;
    private Shared.Dto.Modules.MasterData.Lenders.GetLender.LenderItem? _selectedLender;
    private UpdateLenderCommand? _model;

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
            CommonBreadcrumbFor.Active(LendersDisplayTextFor.Lenders)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetLendersQuery();
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

    private bool FilterItems(LenderItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Name.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Country.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private void ShowDrawerAdd()
    {
        _isDrawerAddOpen = true;
    }

    private async Task ShowDrawerDetails(Guid lenderId)
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetLenderQuery
            {
                LenderId = lenderId
            };

            var response = await Sender.Send(query);

            _selectedLender = response.Item;
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

    private void ShowDrawerEdit(LenderItem item)
    {
        _model = new UpdateLenderCommand
        {
            LenderId = item.Id,
            Name = item.Name,
            Address = item.Address,
            CountryId = item.CountryId,
            PhoneNumber = item.PhoneNumber,
            EmailAddress = item.EmailAddress,
            Website = item.Website
        };

        _isDrawerEditOpen = true;
    }

    private async Task ShowDialogDelete(LenderItem item)
    {
        var model = new DialogDeleteModel
        {
            LenderId = item.Id,
            LenderName = item.Name
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {LendersDisplayTextFor.Lender}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItems();
        }
    }
}
