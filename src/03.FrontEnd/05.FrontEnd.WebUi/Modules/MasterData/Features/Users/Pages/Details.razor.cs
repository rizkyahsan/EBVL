using EBVL.FrontEnd.Logics.Modules.MasterData.Users.GetUser;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;
using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private UserItem _item = default!;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = UsersDisplayTextFor.User;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            MasterDataUsersBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetUserQuery
            {
                UserId = Id
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
            UserId = _item.Id,
            Username = _item.Username
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {UsersDisplayTextFor.User}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            NavigationManager.NavigateTo(MasterDataUsersRouteFor.Index);
        }
    }
}
