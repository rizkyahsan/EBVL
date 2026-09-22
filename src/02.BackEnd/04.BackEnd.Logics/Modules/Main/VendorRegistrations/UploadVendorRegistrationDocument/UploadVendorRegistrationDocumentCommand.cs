using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UploadVendorRegistrationDocument;

public sealed record UploadVendorRegistrationDocumentCommand(Guid RegistrationId, string DefinitionKey, string ResumeToken, FileItem File) : IRequest<UploadVendorRegistrationDocumentResponse>;

public sealed class UploadVendorRegistrationDocumentHandler(IDatabaseService databaseService, IFileStorageDbService storage) : IRequestHandler<UploadVendorRegistrationDocumentCommand, UploadVendorRegistrationDocumentResponse>
{
    public async Task<UploadVendorRegistrationDocumentResponse> Handle(UploadVendorRegistrationDocumentCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        databaseService.SetQuestionnaireRuntimeGraphUnchanged();
        var definition = await databaseService.DocumentDefinitions.SingleOrDefaultAsync(x => !x.IsDeleted && x.IsActive && x.DocumentRequirementSetId == registration.DocumentRequirementSetId && x.Code == request.DefinitionKey, cancellationToken)
            ?? throw new ValidationException("The document definition is invalid.");
        QuestionnaireRuntime.ValidatePdf(request.File, definition.MaxSizeMb);
        var stored = await storage.CreateAsync(request.File, cancellationToken);
        var current = registration.Documents.SingleOrDefault(x => !x.IsDeleted && x.DocumentDefinitionId == definition.Id);
        if (current is { } existingDocument)
        {
            existingDocument.IsDeleted = true;
        }

        var document = new VendorRegistrationDocument { Id = Guid.CreateVersion7(), VendorRegistrationId = registration.Id, DocumentDefinitionId = definition.Id, DefinitionKey = definition.Code, Name = definition.Name, Order = definition.Order, MaxSizeMb = definition.MaxSizeMb, IsMandatory = definition.IsMandatory, FileStorageId = stored.Id, OriginalFileName = Path.GetFileName(request.File.FileName), ContentType = "application/pdf", Length = request.File.FileContent.LongLength };
        _ = databaseService.VendorRegistrationDocuments.Add(document);
        _ = await databaseService.SaveAsync(nameof(UploadVendorRegistrationDocumentCommand), cancellationToken);
        if (current is not null)
        {
            await storage.DeleteAsync(current.FileStorageId, cancellationToken);
        }

        return new(new(definition.Id, definition.Code, definition.Name, definition.Order, definition.IsMandatory, definition.MaxSizeMb, document.Id, document.OriginalFileName, document.ContentType, document.Length), Convert.ToBase64String(registration.RowVersion), await QuestionnaireRuntime.IsDocumentEvidenceComplete(databaseService, registration, cancellationToken));
    }
}
