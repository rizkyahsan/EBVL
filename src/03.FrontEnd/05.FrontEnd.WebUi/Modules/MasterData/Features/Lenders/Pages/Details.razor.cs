using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLender;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;
using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private LenderItem _item = default!;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = LendersDisplayTextFor.Lender;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            MasterDataLendersBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetLenderQuery
            {
                LenderId = Id
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
            LenderId = _item.Id,
            LenderName = _item.Name
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {LendersDisplayTextFor.Lender}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            NavigationManager.NavigateTo(MasterDataLendersRouteFor.Index);
        }
    }
}
