namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Pages;

public partial class Login
{
    [SupplyParameterFromQuery]
    public string? Error { get; set; }

    [SupplyParameterFromQuery]
    public string? Email { get; set; }

    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }
}
