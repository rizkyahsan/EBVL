using EBVL.FrontEnd.Logics.Modules.MasterData.Users.DeleteUser;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;

public partial class DialogDelete
{
    [Parameter]
    public required DialogDeleteModel Model { get; set; }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new DeleteUserCommand
            {
                UserId = Model.UserId
            };

            await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Deleted(UsersDisplayTextFor.User, Model.Username));
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

public sealed record DialogDeleteModel
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}
