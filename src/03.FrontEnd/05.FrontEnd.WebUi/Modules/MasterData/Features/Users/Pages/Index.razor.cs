using EBVL.FrontEnd.Logics.Modules.MasterData.Users.GetUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.GetUsers;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendResetPasswordUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.SendVerificationUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.UpdateUser;
using EBVL.FrontEnd.Logics.Modules.MasterData.Users.UpdateUserPic;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;
using EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Pages;

public partial class Index
{
    private IEnumerable<UserItem> _items = [];
    private string? _searchKeyword;
    private bool _isDrawerAddOpen = false;
    private bool _isDrawerDetailsOpen = false;
    private bool _isDrawerEditOpen = false;
    private Shared.Dto.Modules.MasterData.Users.GetUser.UserItem? _selectedUser;
    private UpdateUserCommand? _model;

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
            CommonBreadcrumbFor.Active(UsersDisplayTextFor.Users)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetUsersQuery();
            var response = await Sender.Send(query);

            _items = response.Items;
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

    private bool FilterItems(UserItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        //if (item.Name.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        //{
        //    return true;
        //}

        if (item.LenderName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private void ShowDrawerAdd()
    {
        _isDrawerAddOpen = true;
    }

    private async Task ShowDrawerDetails(Guid userId)
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetUserQuery
            {
                UserId = userId
            };

            var response = await Sender.Send(query);

            _selectedUser = response.Item;
            _isDrawerDetailsOpen = true;
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

    private void ShowDrawerEdit(UserItem item)
    {
        _model = new UpdateUserCommand
        {
            UserId = item.Id,
            LenderId = item.LenderId,
            LenderName = item.LenderName,
            Name = item.Name,
            PhoneNumber = item.PhoneNumber,
            CountryPhoneCode = item.PhoneCode,
            EmailAddress = item.EmailAddress,
        };

        _isDrawerEditOpen = true;
    }

    private async Task ShowDialogDelete(UserItem item)
    {
        var model = new DialogDeleteModel
        {
            UserId = item.Id,
            Username = item.Name
        };

        var parameters = new DialogParameters<DialogDelete>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogDelete>($"{CommonDisplayTextFor.Delete} {UsersDisplayTextFor.User}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItems();
        }
    }

    private async Task SendVerificationEmail(UserItem item)
    {
        try
        {
            _isLoading = true;

            ClearException();
            await InvokeAsync(StateHasChanged);
            var command = new SendVerificationUserCommand
            {
                UserId = item.Id
            };

            var response = await Sender.Send(command);

            Snackbar.AddSuccess(response.Item.Message);
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task SendResetPasswordEmail(UserItem item)
    {
        try
        {
            _isLoading = true;

            ClearException();
            await InvokeAsync(StateHasChanged);
            var command = new SendResetPasswordUserCommand
            {
                UserId = item.Id
            };

            var response = await Sender.Send(command);

            Snackbar.AddSuccess(response.Item.Message);
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task UpdateLenderPic(UserItem item, bool isPic)
    {
        try
        {
            _isLoading = true;

            ClearException();

            item.IsPicLender = isPic;

            var command = new UpdateUserPicCommand
            {
                UserId = item.Id,
                IsPic = isPic
            };

            await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Action(UsersDisplayTextFor.Username, item.Name, "Update Pic"));

            await LoadItems();
        }
        catch (Exception exception)
        {
            // Revert UI if update failed
            item.IsPicLender = !isPic;
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
