using EBVL.FrontEnd.Logics.Modules.MasterData.Users.UpdateUser;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;

public partial class DrawerEdit
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    [Parameter, EditorRequired]
    public required UpdateUserCommand? Model { get; init; }

    private MudForm _form = default!;
    private UpdateUserCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _validator = new();
    }

    private async Task Submit()
    {
        ClearException();

        if (Model is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            await Sender.Send(Model);

            Snackbar.AddSuccess(SuccessMessageFor.Updated(UsersDisplayTextFor.User, Model.Name));

            await IsOpenChanged.InvokeAsync(false);
            await OnReload.InvokeAsync();
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
