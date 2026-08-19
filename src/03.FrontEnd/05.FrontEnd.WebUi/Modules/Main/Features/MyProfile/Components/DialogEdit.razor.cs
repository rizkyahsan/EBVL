using EBVL.FrontEnd.Logics.Modules.Main.Users.UpdateMyUser;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;

public partial class DialogEdit
{
    [Parameter]
    public required UpdateMyUserCommand Model { get; init; }

    private MudForm _form = default!;
    private UpdateMyUserCommandValidator _validator = default!;

    protected override void OnParametersSet()
    {
        _validator = new();
    }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            await _form.RunValidation();
            await Sender.Send(Model);

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
