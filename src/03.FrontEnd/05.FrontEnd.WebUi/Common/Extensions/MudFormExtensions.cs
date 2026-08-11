namespace EBVL.FrontEnd.WebUi.Common.Extensions;

public static class MudFormExtensions
{
    public static async Task RunValidation(this MudForm form)
    {
        await form.Validate();

        if (!form.IsValid)
        {
            throw ExceptionFor.FormValidation(form.Errors);
        }
    }
}
