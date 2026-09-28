using EBVL.FrontEnd.Logics.Modules.Main.Users.VerifyMyUser;
using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;

public partial class DialogVerify : DialogBase
{
    private MudForm _form = default!;
    private VerifyMyUserCommand _model = default!;
    private VerifyMyUserCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            VerificationCode = string.Empty
        };

        _validator = new();
    }

    protected async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            await _form.RunValidation();

            await Sender.Send(_model);

            Snackbar.AddSuccess($"{CommonDisplayTextFor.Your} {UsersDisplayTextFor.UserProfile} has been successfully {CommonDisplayTextFor.Verified.ToLower()}.");

            Dialog.Close();
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
}
