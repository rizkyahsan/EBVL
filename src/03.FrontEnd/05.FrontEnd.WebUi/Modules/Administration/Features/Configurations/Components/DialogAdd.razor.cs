using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.AddConfiguration;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Components;

public partial class DialogAdd
{
    private MudForm _form = default!;
    private AddConfigurationCommand _model = default!;
    private AddConfigurationCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            Key = string.Empty,
            Value = string.Empty
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

            var response = await Sender.Send(_model);

            Snackbar.AddSuccess(SuccessMessageFor.Added(ConfigurationsDisplayTextFor.Configuration, _model.Key));

            Dialog.Close(DialogResult.Ok(response.Item.Id));
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
