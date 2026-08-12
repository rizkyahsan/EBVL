using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;
using System.Diagnostics.CodeAnalysis;

namespace EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed record RegisterVendorCommand : RegisterVendorRequest, IRequest<RegisterVendorResponse>
{
    [SetsRequiredMembers]
    public RegisterVendorCommand(RegisterVendorRequest request)
    {
        SapVendorNumber = request.SapVendorNumber;
        CompanyName = request.CompanyName;
        CompanyEmail = request.CompanyEmail;
        PicEmail = request.PicEmail;
        CompanyPhone = request.CompanyPhone;
        PicPhone = request.PicPhone;
        Website = request.Website;
        CompanyService = request.CompanyService;
        FactoryCountry = request.FactoryCountry;
        FactoryAddress = request.FactoryAddress;
        BrandRepresentative = request.BrandRepresentative;
        CompanyStatus = request.CompanyStatus;
        HasRepresentativeInIndonesia = request.HasRepresentativeInIndonesia;
        IndonesiaRepresentativeName = request.IndonesiaRepresentativeName;
        BrandRegistrationLetterFileName = request.BrandRegistrationLetterFileName;
        CompanyProfileFileName = request.CompanyProfileFileName;
        ProductCatalogFileName = request.ProductCatalogFileName;
        ProjectExperienceFileName = request.ProjectExperienceFileName;
        TaxCardFileName = request.TaxCardFileName;
        MainCertificateFileName = request.MainCertificateFileName;
        Password = request.Password;
        PasswordConfirmation = request.PasswordConfirmation;
    }
}

public sealed class RegisterVendorCommandValidator : AbstractValidatorBase<RegisterVendorCommand>
{
    public RegisterVendorCommandValidator()
    {
        Include(new RegisterVendorRequestValidator());
    }
}

public sealed class RegisterVendorCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<RegisterVendorCommand, RegisterVendorResponse>
{
    public async Task<RegisterVendorResponse> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        var email = request.CompanyEmail.Trim().ToLowerInvariant();
        if (await databaseService.VendorAccounts.AnyAsync(x => !x.IsDeleted && x.EmailAddress == email, cancellationToken))
        {
            throw new ModelValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.CompanyEmail)] = ["Email perusahaan sudah terdaftar."]
            });
        }

        if (await databaseService.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.SapVendorNumber == request.SapVendorNumber, cancellationToken))
        {
            throw new ModelValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.SapVendorNumber)] = ["Nomor SAP Vendor sudah terdaftar."]
            });
        }

        var registration = new VendorRegistration
        {
            SapVendorNumber = request.SapVendorNumber.Trim(),
            CompanyName = request.CompanyName.Trim(),
            CompanyEmail = email,
            PicEmail = request.PicEmail.Trim().ToLowerInvariant(),
            CompanyPhone = request.CompanyPhone.Trim(),
            PicPhone = request.PicPhone.Trim(),
            Website = request.Website?.Trim(),
            CompanyService = request.CompanyService.Trim(),
            FactoryCountry = request.FactoryCountry.Trim(),
            FactoryAddress = request.FactoryAddress.Trim(),
            BrandRepresentative = request.BrandRepresentative.Trim(),
            CompanyStatus = request.CompanyStatus.Trim(),
            HasRepresentativeInIndonesia = request.HasRepresentativeInIndonesia,
            IndonesiaRepresentativeName = request.IndonesiaRepresentativeName?.Trim(),
            BrandRegistrationLetterFileName = request.BrandRegistrationLetterFileName,
            CompanyProfileFileName = request.CompanyProfileFileName,
            ProductCatalogFileName = request.ProductCatalogFileName,
            ProjectExperienceFileName = request.ProjectExperienceFileName,
            TaxCardFileName = request.TaxCardFileName,
            MainCertificateFileName = request.MainCertificateFileName,
            Status = VendorRegistrationStatus.Submitted
        };

        var (passwordHash, passwordSalt) = PasswordHasher.Hash(request.Password);
        var account = new VendorAccount
        {
            EmailAddress = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            VendorRegistrationId = registration.Id,
            VendorRegistration = registration
        };

        _ = await databaseService.VendorRegistrations.AddAsync(registration, cancellationToken);
        _ = await databaseService.VendorAccounts.AddAsync(account, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(RegisterVendor), cancellationToken);

        return new RegisterVendorResponse
        {
            VendorRegistrationId = registration.Id,
            CompanyName = registration.CompanyName,
            Status = registration.Status
        };
    }
}
