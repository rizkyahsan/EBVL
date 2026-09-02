using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.SapVendor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class CompanyProfileForm
{
    [Parameter]
    public required PreRegistrationRequest Model { get; init; }

    [Parameter]
    public required EventCallback<PreRegistrationRequest> OnNext { get; init; }

    [Parameter]
    public EventCallback<string> OnSapFound { get; init; }

    private EditForm _form = default!;
    private MudForm _sapForm = default!;
    private readonly SapVendorRequest _sapModel = new();
    private readonly SapVendorRequestValidator _sapValidator = new();
    private bool _sapExpanded = true;
    private bool _profileExpanded;
    private bool _sapFound;
    private bool _isFindingSap;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Model.SapVendorNumber) || _sapFound)
        {
            return;
        }

        _sapModel.SapVendorNumber = Model.SapVendorNumber;
        ShowCompanyProfile();
    }

    private async Task FindSapVendor()
    {
        if (_isFindingSap)
        {
            return;
        }

        _isFindingSap = true;
        try
        {
            await _sapForm.Validate();
            if (!_sapForm.IsValid)
            {
                return;
            }

            await OnSapFound.InvokeAsync(_sapModel.SapVendorNumber);
            ShowCompanyProfile();
        }
        finally
        {
            _isFindingSap = false;
        }
    }

    private void ShowCompanyProfile()
    {
        _sapFound = true;
        _sapExpanded = false;
        _profileExpanded = true;
    }

    private void SetSapExpanded(bool expanded)
    {
        _sapExpanded = expanded;
    }

    private void SetProfileExpanded(bool expanded)
    {
        _profileExpanded = expanded;
    }

    private void AddBrand()
    {
        Model.AdditionalBrands.Add(string.Empty);
    }

    private void UpdateBrand(int index, string? value)
    {
        Model.AdditionalBrands[index] = value ?? string.Empty;
    }

    private void RemoveBrand(int index)
    {
        Model.AdditionalBrands.RemoveAt(index);
    }

    public async Task SubmitAsync()
    {
        if (_form.EditContext?.Validate() is true)
        {
            await OnNext.InvokeAsync(Model);
        }
    }
}
