using EBVL.FrontEnd.Logics.Modules.Examples.PublicHolidays.LoadPublicHolidays;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.PublicHolidays.Components;

public partial class DialogLoadPublicHolidays
{
    private readonly LoadPublicHolidaysCommand _command = new()
    {
        Year = DateTime.Now.Year,
        CountryCode = "ID"
    };

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            _ = await Sender.Send(_command);
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
