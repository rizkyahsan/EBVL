using EBVL.FrontEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;
using EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Components.Dialog;
using EBVL.FrontEnd.Services.BackEndApi;
using RestSharp;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.VendorRegistrations.Pages;

public partial class Register
{
    [Inject] public required ILogger<Register> Logger { get; init; }
    [Inject] public required IDialogService DialogService { get; init; }
    [Inject] public required IBackEndApiService BackEndApiService { get; init; }

    private int _step;
    private bool _isLoading;
    private Exception? _exception;
    private string? _validationMessage;
    private bool _filesResetAfterBack;
    private readonly IBrowserFile?[] _files = new IBrowserFile?[6];
    private const long MaxFileSize = 20_971_520;
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
        if (_step == 3)
        {
            Array.Clear(_files);
            ClearFileNames();
            _filesResetAfterBack = true;
        }

        _step = Math.Max(0, _step - 1);
    }

    private void RegisterClick()
    {
        Logger.LogDebug("REGISTER_CLICK_ENTERED");
    }

    private void SetFile(InputFileChangeEventArgs args, int slot)
    {
        _filesResetAfterBack = false;
        var file = args.File;
        _files[slot - 1] = file;
        var name = file.Name;
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
        Logger.LogDebug("REGISTER_SUBMIT_HANDLER_ENTERED");
        if (_isLoading)
        {
            return;
        }

        _validationMessage = ValidateForm();
        if (_validationMessage is not null)
        {
            return;
        }

        try
        {
            _isLoading = true;
            _exception = null;
            var dialog = await DialogService.ShowAsync<ConfirmSubmissionDialog>();
            var result = await dialog.Result;
            if (result is null || result.Canceled)
            {
                return;
            }

            var request = new RestRequest(Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor.RegisterVendorRoute.ResourceUri, Method.Post)
            {
                AlwaysMultipartFormData = true
            };
            AddFormFields(request);
            for (var index = 0; index < _files.Length; index++)
            {
                var file = _files[index]!;
                _ = request.AddFile(_fileFieldNames[index], await file.ToBytesAsync(MaxFileSize), file.Name, file.ContentType);
            }

            var response = await BackEndApiService.SendAnonymousRequestAsync<Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor.RegisterVendorResponse>(request);
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

    private void HandleInvalidSubmit(EditContext _)
    {
        Logger.LogDebug("REGISTER_SUBMIT_FORM_INVALID");
        _validationMessage = "Periksa kembali field wajib, password, dan keenam dokumen sebelum mengirim pendaftaran.";
    }

    private string? ValidateForm()
    {
        var missing = Enumerable.Range(0, _files.Length).Where(index => _files[index] is null).Select(index => index + 1).ToArray();
        if (missing.Length > 0)
        {
            return $"Dokumen wajib belum dipilih: {string.Join(", ", missing)}.";
        }

        if (_files.Any(file => file!.Size > MaxFileSize || !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase)))
        {
            return "Setiap dokumen wajib harus berupa PDF maksimal 20 MB.";
        }

        if (string.IsNullOrWhiteSpace(_model.Password) || _model.Password.Length < 12 || !HasUpperLowerDigit(_model.Password))
        {
            return "Password minimal 12 karakter dan harus mengandung huruf besar, huruf kecil, serta angka.";
        }

        return _model.Password != _model.PasswordConfirmation ? "Konfirmasi password tidak sama." : null;
    }

    private static bool IsValidFile(IBrowserFile? file)
    {
        return file is not null
            && file.Size <= MaxFileSize
            && string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)
            && string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase);
    }

    private void ClearFileNames()
    {
        _model.BrandRegistrationLetterFileName = string.Empty;
        _model.CompanyProfileFileName = string.Empty;
        _model.ProductCatalogFileName = string.Empty;
        _model.ProjectExperienceFileName = string.Empty;
        _model.TaxCardFileName = string.Empty;
        _model.MainCertificateFileName = string.Empty;
    }

    private static bool HasUpperLowerDigit(string value)
    {
        return value.Any(char.IsUpper) && value.Any(char.IsLower) && value.Any(char.IsDigit);
    }

    private void AddFormFields(RestRequest request)
    {
        foreach (var property in typeof(RegisterVendorCommand).GetProperties())
        {
            var value = property.GetValue(_model)?.ToString() ?? string.Empty;
            _ = request.AddParameter(property.Name, value);
        }
    }

    private string FileLabel(int slot)
    {
        return _files[slot - 1]?.Name ?? "Belum dipilih";
    }

    private static readonly string[] _fileFieldNames = ["brandRegistrationLetter", "companyProfile", "productCatalog", "projectExperience", "taxCard", "mainCertificate"];
}
