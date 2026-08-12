using EBVL.FrontEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;
using EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Components.Dialog;
using MediatR;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Pages;

public partial class Register
{
    [Inject] public required ISender Sender { get; init; }
    [Inject] public required IDialogService DialogService { get; init; }

    private int _step;
    private bool _isLoading;
    private Exception? _exception;
    private string? _resultCompany;
    private readonly RegisterVendorCommand _model = new()
    {
        SapVendorNumber = string.Empty,
        CompanyName = string.Empty,
        CompanyEmail = string.Empty,
        PicEmail = string.Empty,
        CompanyPhone = string.Empty,
        PicPhone = string.Empty,
        CompanyService = string.Empty,
        FactoryCountry = string.Empty,
        FactoryAddress = string.Empty,
        BrandRepresentative = string.Empty,
        CompanyStatus = string.Empty,
        HasRepresentativeInIndonesia = false,
        BrandRegistrationLetterFileName = string.Empty,
        CompanyProfileFileName = string.Empty,
        ProductCatalogFileName = string.Empty,
        ProjectExperienceFileName = string.Empty,
        TaxCardFileName = string.Empty,
        MainCertificateFileName = string.Empty,
        Password = string.Empty,
        PasswordConfirmation = string.Empty
    };

    private void Next()
    {
        _step = Math.Min(3, _step + 1);
    }

    private void Back()
    {
        _step = Math.Max(0, _step - 1);
    }

    private void SetFile(InputFileChangeEventArgs args, int slot)
    {
        var name = args.File.Name;
        switch (slot)
        {
            case 1:
                _model.BrandRegistrationLetterFileName = name;
                break;
            case 2:
                _model.CompanyProfileFileName = name;
                break;
            case 3:
                _model.ProductCatalogFileName = name;
                break;
            case 4:
                _model.ProjectExperienceFileName = name;
                break;
            case 5:
                _model.TaxCardFileName = name;
                break;
            case 6:
                _model.MainCertificateFileName = name;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "Slot dokumen tidak dikenal.");
        }
    }

    private async Task SubmitRegistration()
    {
        var dialog = await DialogService.ShowAsync<ConfirmSubmissionDialog>();
        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        try
        {
            _isLoading = true;
            _exception = null;
            var response = await Sender.Send(_model);
            _resultCompany = response.CompanyName;
            _step = 4;
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
