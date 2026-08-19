using EBVL.FrontEnd.Logics.Modules.Main.Users.GetMyUser;
using EBVL.FrontEnd.Logics.Modules.Main.Users.SendMyVerificationCode;
using EBVL.FrontEnd.Logics.Modules.Main.Users.UpdateMyUser;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;
using EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Pages;

public partial class Index
{
    private IndexModel _model = default!;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadModel();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            CommonBreadcrumbFor.Active(UsersDisplayTextFor.MyProfile)
        ];
    }

    private async Task LoadModel()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetMyUserQuery();
            var response = await Sender.Send(query);

            _model = new IndexModel
            {
                User = response.Item
            };
        }
        catch (EntityNotFoundException)
        {
            _model = new IndexModel
            {
                User = null
            };
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

    private async Task ShowDialogCreate()
    {
        ClearException();

        var dialog = await DialogService.ShowAsync<DialogCreate>($"{CommonDisplayTextFor.Create} {UsersDisplayTextFor.MyProfile}");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadModel();
        }
    }

    private async Task ShowDialogViewQrCode()
    {
        ClearException();

        if (_model.User is null)
        {
            return;
        }

        var parameters = new DialogParameters<DialogViewQrCode>
        {
            { x => x.DataUri, _model.User.QrCodeDataUri }
        };

        _ = await DialogService.ShowAsync<DialogViewQrCode>($"{UsersDisplayTextFor.MyProfile} QR Code", parameters);
    }

    private async Task SendMyVerificationCode()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new SendMyVerificationCodeCommand();
            var response = await Sender.Send(command);

            Snackbar.AddSuccess(response.Item.Message);
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

    private async Task ShowDialogVerify()
    {
        ClearException();

        var dialog = await DialogService.ShowAsync<DialogVerify>($"{CommonDisplayTextFor.Verify} {UsersDisplayTextFor.MyProfile}");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadModel();
        }
    }

    private async Task ShowDialogEdit()
    {
        ClearException();

        if (_model.User is null)
        {
            return;
        }

        var model = new UpdateMyUserCommand
        {
            Name = _model.User.Name,
            EmailAddress = _model.User.EmailAddress,
            PhoneNumber = _model.User.PhoneNumber,
            VerificationCode = string.Empty
        };

        var parameters = new DialogParameters<DialogEdit>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogEdit>($"{CommonDisplayTextFor.Edit} {UsersDisplayTextFor.Profile}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadModel();
        }
    }

    private async Task ShowDialogReload()
    {
        ClearException();

        var dialog = await DialogService.ShowAsync<DialogReload>($"{CommonDisplayTextFor.Reload} {UsersDisplayTextFor.MyProfile}");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadModel();
        }
    }

    private sealed record IndexModel
    {
        public required UserItem? User { get; set; }
    }
}
