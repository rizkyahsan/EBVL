using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;
using System.Diagnostics.CodeAnalysis;
using Pertamina.Services.FileStorage;

namespace EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed record RegisterVendorCommand : RegisterVendorRequest, IRequest<RegisterVendorResponse>
{
    public Guid CorrelationId { get; init; } = Guid.CreateVersion7();
    public required VendorRegistrationFileItem BrandRegistrationLetter { get; init; } = null!;
    public required VendorRegistrationFileItem CompanyProfile { get; init; } = null!;
    public required VendorRegistrationFileItem ProductCatalog { get; init; } = null!;
    public required VendorRegistrationFileItem ProductExperienceList { get; init; } = null!;
    public required VendorRegistrationFileItem NpwpPerusahaan { get; init; } = null!;
    public required VendorRegistrationFileItem BrandCertificate { get; init; } = null!;
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
        BrandRegistrationLetterFileName = request.BrandRegistrationLetterFileName ?? string.Empty;
        CompanyProfileFileName = request.CompanyProfileFileName ?? string.Empty;
        ProductCatalogFileName = request.ProductCatalogFileName ?? string.Empty;
        ProjectExperienceFileName = request.ProjectExperienceFileName ?? string.Empty;
        TaxCardFileName = request.TaxCardFileName ?? string.Empty;
        MainCertificateFileName = request.MainCertificateFileName ?? string.Empty;
        Password = request.Password;
        PasswordConfirmation = request.PasswordConfirmation;
    }
}

public sealed class RegisterVendorCommandValidator : AbstractValidatorBase<RegisterVendorCommand>
{
    public RegisterVendorCommandValidator()
    {
        Include(new RegisterVendorRequestValidator());
        _ = RuleFor(x => x.BrandRegistrationLetter).NotNull();
        _ = RuleFor(x => x.CompanyProfile).NotNull();
        _ = RuleFor(x => x.ProductCatalog).NotNull();
        _ = RuleFor(x => x.ProductExperienceList).NotNull();
        _ = RuleFor(x => x.NpwpPerusahaan).NotNull();
        _ = RuleFor(x => x.BrandCertificate).NotNull();
    }
}

public sealed class RegisterVendorCommandHandler(
    IDatabaseService databaseService,
    IFileStorageService fileStorageService)
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

        var sapVendorNumber = request.SapVendorNumber.Trim();
        if (await databaseService.Vendors.AnyAsync(x => !x.IsDeleted && x.SapVendorNumber == sapVendorNumber, cancellationToken))
        {
            throw new ModelValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.SapVendorNumber)] = ["Nomor SAP Vendor sudah terdaftar di master vendor."]
            });
        }

        var vendor = new Vendor
        {
            SapVendorNumber = sapVendorNumber,
            Name = request.CompanyName.Trim(),
            Email = email,
            Website = request.Website?.Trim()
        };

        var registration = new VendorRegistration
        {
            Vendor = vendor,
            VendorId = vendor.Id,
            SapVendorNumber = sapVendorNumber,
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
            BrandRegistrationLetterFileName = request.BrandRegistrationLetterFileName ?? request.BrandRegistrationLetter.FileName,
            CompanyProfileFileName = request.CompanyProfileFileName ?? request.CompanyProfile.FileName,
            ProductCatalogFileName = request.ProductCatalogFileName ?? request.ProductCatalog.FileName,
            ProjectExperienceFileName = request.ProjectExperienceFileName ?? request.ProductExperienceList.FileName,
            TaxCardFileName = request.TaxCardFileName ?? request.NpwpPerusahaan.FileName,
            MainCertificateFileName = request.MainCertificateFileName ?? request.BrandCertificate.FileName,
            Status = VendorRegistrationStatus.Submitted
        };

        var (passwordHash, passwordSalt) = PasswordHasher.Hash(request.Password);
        var account = new VendorAccount
        {
            EmailAddress = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = false,
            Status = VendorAccountStatus.PendingActivation,
            VendorId = vendor.Id,
            VendorRegistrationId = registration.Id,
            VendorRegistration = registration,
            Vendor = vendor
        };

        var documentFiles = new[]
        {
            ("Brand Registration Letter", request.BrandRegistrationLetter),
            ("Company Profile", request.CompanyProfile),
            ("Product Catalog", request.ProductCatalog),
            ("Product Experience List", request.ProductExperienceList),
            ("Tax Identification Number / NPWP", request.NpwpPerusahaan),
            ("Brand Certificate", request.BrandCertificate)
        };
        var templateRows = await databaseService.DocumentTemplates
            .Where(x => !x.IsDeleted && documentFiles.Select(file => file.Item1).Contains(x.Name))
            .ToListAsync(cancellationToken);
        var templates = templateRows
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        if (templates.Count != documentFiles.Length)
        {
            throw new InvalidOperationException("One or more registration document templates are not configured.");
        }

        var registrationDocuments = new List<(VendorRegistrationDocument Entity, string Path, byte[] Content)>();
        var vendorDocuments = new List<VendorDocument>();
        foreach (var (templateName, file) in documentFiles)
        {
            var storedFileName = $"{Guid.CreateVersion7()}{Path.GetExtension(file.FileName)}";
            var storagePath = Path.Combine("vendor-registrations", registration.Id.ToString("N"), storedFileName);
            registrationDocuments.Add((new VendorRegistrationDocument
            {
                VendorRegistrationId = registration.Id,
                DocumentTemplateId = templates[templateName].Id,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FileContentType = file.ContentType,
                FileSize = file.FileContent.LongLength,
                StorageFileId = storagePath,
                IsVerified = false,
                VendorRegistration = registration,
                DocumentTemplate = templates[templateName]
            }, storagePath, file.FileContent));
            vendorDocuments.Add(new VendorDocument
            {
                VendorId = vendor.Id,
                Vendor = vendor,
                DocumentTemplateId = templates[templateName].Id,
                DocumentTemplate = templates[templateName],
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FileContentType = file.ContentType,
                FileSize = file.FileContent.LongLength,
                StorageFileId = storagePath,
                IsVerified = false
            });
        }

        await using var transaction = await databaseService.BeginTransactionAsync(cancellationToken);
        var storedPaths = new List<string>();
        try
        {
            _ = await databaseService.Vendors.AddAsync(vendor, cancellationToken);
            _ = await databaseService.SaveAsync(nameof(RegisterVendor), cancellationToken);

            _ = await databaseService.VendorRegistrations.AddAsync(registration, cancellationToken);
            _ = await databaseService.VendorAccounts.AddAsync(account, cancellationToken);
            await databaseService.VendorRegistrationDocuments.AddRangeAsync(registrationDocuments.Select(x => x.Entity), cancellationToken);
            await databaseService.VendorDocuments.AddRangeAsync(vendorDocuments, cancellationToken);
            _ = await databaseService.SaveAsync(nameof(RegisterVendor), cancellationToken);

            foreach (var document in registrationDocuments)
            {
                await fileStorageService.CreateAsync(document.Path, document.Content, cancellationToken);
                storedPaths.Add(document.Path);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            foreach (var path in storedPaths)
            {
                try
                {
                    await fileStorageService.DeleteAsync(path, CancellationToken.None);
                }
                catch
                {
                    // Preserve the original failure; storage cleanup is best effort.
                }
            }

            throw;
        }

        return new RegisterVendorResponse
        {
            VendorRegistrationId = registration.Id,
            VendorId = vendor.Id,
            CompanyName = registration.CompanyName,
            Status = registration.Status,
            DocumentCount = vendorDocuments.Count,
            CorrelationId = request.CorrelationId
        };
    }
}
