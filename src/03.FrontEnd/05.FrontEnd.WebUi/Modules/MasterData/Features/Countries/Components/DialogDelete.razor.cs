using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.DeleteCountry;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;

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

            var command = new DeleteCountryCommand
            {
                CountryId = Model.CountryId
            };

            await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Deleted(CountriesDisplayTextFor.Country, Model.CountryName));
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

public sealed record DialogDeleteModel
{
    public required Guid CountryId { get; init; }
    public required string CountryName { get; init; }
}
