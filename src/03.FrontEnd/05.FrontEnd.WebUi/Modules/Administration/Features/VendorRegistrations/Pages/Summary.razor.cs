using System.Security.Claims;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Pages;

public partial class Summary
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;
    private string _companyName = string.Empty;
    private string _email = string.Empty;
    private string _sapVendorNumber = string.Empty;
    private string _status = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthenticationStateTask).User;
        _companyName = user.Identity?.Name ?? "Vendor";
        _email = user.FindFirstValue(ClaimTypes.Email) ?? "-";
        _sapVendorNumber = user.FindFirstValue("SapVendorNumber") ?? "-";
        _status = user.FindFirstValue("VendorRegistrationStatus") ?? "-";
    }
}
