using EBVL.FrontEnd.Logics.Modules.MasterData.Users.ForgotPasswordUser;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class ForgotPassword
{
    [Inject] public required ISender Sender { get; init; }

    private readonly ForgotPasswordUserCommandValidator _validator = new();
    private readonly ForgotPasswordUserCommand _model = new() { UsernameOrEmail = string.Empty };
    private MudForm _form = default!;
    private bool _isLoading;
    private bool _isSuccess;
    private string _successMessage = string.Empty;
    protected Exception? _exception;

    private async Task ExecuteRequest()
    {
        if (_isLoading)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        try
        {
            _isLoading = true;
            _exception = null;
            var response = await Sender.Send(_model);
            _successMessage = response.Item.Message;
            _isSuccess = true;
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
