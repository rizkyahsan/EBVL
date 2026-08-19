using EBVL.FrontEnd.Logics.Modules.Main.Users.CreateMyUser;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProfile.Components;

public partial class DialogCreate
{
    private bool _confirm;
    private static readonly string _labelConfirm = $"I hereby declare that I agree to {CommonDisplayTextFor.Create.ToLower()} a {UsersDisplayTextFor.Profile} in this application.";

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            if (!_confirm)
            {
                Dialog.Close();

                return;
            }

            var command = new CreateMyUserCommand();
            var response = await Sender.Send(command);

            //var command = new AddUserCommand();
            //var response = await Sender.Send(command);

            Snackbar.AddSuccess($"{CommonDisplayTextFor.Your} {UsersDisplayTextFor.UserProfile} ({response.Item.Username}) has been successfully {CommonDisplayTextFor.Created.ToLower()}.");

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
