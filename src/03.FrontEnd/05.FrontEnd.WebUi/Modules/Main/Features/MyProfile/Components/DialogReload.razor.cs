using EBVL.FrontEnd.Logics.Modules.Main.Users.ReloadMyUser;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;

public partial class DialogReload
{
    private MudForm _form = default!;
    private ReloadMyUserCommand _model = default!;
    private ReloadMyUserCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            VerificationCode = string.Empty
        };

        _validator = new();
    }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            await _form.RunValidation();

            await Sender.Send(_model);

            Snackbar.AddSuccess($"{CommonDisplayTextFor.Your} {UsersDisplayTextFor.UserProfile} has been successfully {CommonDisplayTextFor.Reloaded.ToLower()}.");

            Dialog.Close();
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
}
