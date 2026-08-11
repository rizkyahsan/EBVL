using EBVL.FrontEnd.WebUi.Modules.Examples.Models;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Components;

public partial class DialogAddSomething
{
    private MudForm _form = default!;
    private readonly Something _model = new() { Name = FakerFor.English.Name.FullName() };
    private readonly SomethingValidator _validator = new();

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            await _form.RunValidation();
            await Task.Delay(500);

            var somethingResult = new SomethingResult
            {
                Name = _model.Name,
                Code = Guid.CreateVersion7().ToString()
            };

            Dialog.Close(DialogResult.Ok(somethingResult));
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
