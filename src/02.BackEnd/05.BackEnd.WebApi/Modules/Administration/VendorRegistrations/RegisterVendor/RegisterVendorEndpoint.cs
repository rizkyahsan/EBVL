using EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.RegisterVendor;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

namespace EBVL.BackEnd.WebApi.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed class RegisterVendorEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(RegisterVendorRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(RegisterVendorRoute.Name)
            .WithDescription(RegisterVendorRoute.Description)
            .Produces<RegisterVendorResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        [FromForm] RegisterVendorRequest request,
        IFormFile? brandRegistrationLetter,
        IFormFile? companyProfile,
        IFormFile? productCatalog,
        IFormFile? projectExperience,
        IFormFile? taxCard,
        IFormFile? mainCertificate,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (brandRegistrationLetter is null || brandRegistrationLetter.Length == 0
            || companyProfile is null || companyProfile.Length == 0
            || productCatalog is null || productCatalog.Length == 0
            || projectExperience is null || projectExperience.Length == 0
            || taxCard is null || taxCard.Length == 0
            || mainCertificate is null || mainCertificate.Length == 0)
        {
            return Results.Problem("Enam dokumen wajib harus dipilih.", statusCode: StatusCodes.Status400BadRequest);
        }

        var files = new[] { brandRegistrationLetter, companyProfile, productCatalog, projectExperience, taxCard, mainCertificate };
        if (files.Any(x => x.Length > 20_971_520 || !string.Equals(x.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals(Path.GetExtension(x.FileName), ".pdf", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.Problem("Setiap dokumen wajib harus berupa PDF maksimal 20 MB.", statusCode: StatusCodes.Status400BadRequest);
        }

        var command = new RegisterVendorCommand(request)
        {
            BrandRegistrationLetterFileName = request.BrandRegistrationLetterFileName ?? Path.GetFileName(brandRegistrationLetter.FileName),
            CompanyProfileFileName = request.CompanyProfileFileName ?? Path.GetFileName(companyProfile.FileName),
            ProductCatalogFileName = request.ProductCatalogFileName ?? Path.GetFileName(productCatalog.FileName),
            ProjectExperienceFileName = request.ProjectExperienceFileName ?? Path.GetFileName(projectExperience.FileName),
            TaxCardFileName = request.TaxCardFileName ?? Path.GetFileName(taxCard.FileName),
            MainCertificateFileName = request.MainCertificateFileName ?? Path.GetFileName(mainCertificate.FileName),
            BrandRegistrationLetter = await ToFileItemAsync(brandRegistrationLetter, cancellationToken),
            CompanyProfile = await ToFileItemAsync(companyProfile, cancellationToken),
            ProductCatalog = await ToFileItemAsync(productCatalog, cancellationToken),
            ProductExperienceList = await ToFileItemAsync(projectExperience, cancellationToken),
            NpwpPerusahaan = await ToFileItemAsync(taxCard, cancellationToken),
            BrandCertificate = await ToFileItemAsync(mainCertificate, cancellationToken)
        };

        var response = await sender.Send(command, cancellationToken);
        return Results.Created($"{RegisterVendorRoute.ResourceUri}/{response.VendorRegistrationId}", response);
    }

    private static async Task<VendorRegistrationFileItem> ToFileItemAsync(IFormFile file, CancellationToken cancellationToken)
    {
        return new VendorRegistrationFileItem
        {
            FileContent = await file.ToBytesAsync(cancellationToken),
            FileName = Path.GetFileName(file.FileName),
            ContentType = file.ContentType
        };
    }
}
