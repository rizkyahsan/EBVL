using EBVL.FrontEnd.Logics.Modules.MasterData.Users.AddUser;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Users.Components;

public partial class DrawerAdd
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    private MudForm _form = default!;
    private AddUserCommand _model = default!;
    private AddUserCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            LenderId = Guid.Empty,
            Name = string.Empty,
            EmailAddress = string.Empty,
            CountryPhoneCode = string.Empty,
            PhoneNumber = string.Empty
        };

        _validator = new();
    }

    private async Task Submit()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            _ = await Sender.Send(_model);

            Snackbar.AddSuccess(SuccessMessageFor.Added(UsersDisplayTextFor.User, _model.Name));

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
