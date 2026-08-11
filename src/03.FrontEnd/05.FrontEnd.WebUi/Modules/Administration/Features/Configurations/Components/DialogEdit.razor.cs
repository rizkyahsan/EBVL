using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Components;

public partial class DialogEdit
{
    [Parameter]
    public required UpdateConfigurationCommand Model { get; init; }

    private MudForm _form = default!;
    private UpdateConfigurationCommandValidator _validator = default!;

    protected override void OnInitialized()
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

            Snackbar.AddSuccess(SuccessMessageFor.Updated(ConfigurationsDisplayTextFor.Configuration, Model.Key));

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
