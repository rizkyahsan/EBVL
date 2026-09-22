using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EBVL.BackEnd.Logics.Common.Services.FileStorageDb;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;

#region Requests

public sealed record StartQuestionnaireCommand(PreRegistrationRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record GetQuestionnaireRuntimeQuery(Guid RegistrationId, string ResumeToken) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record UpdateVendorRegistrationProfileCommand(Guid RegistrationId, UpdateVendorRegistrationProfileRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record SaveQuestionnaireAnswersCommand(Guid RegistrationId, SaveAnswersRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record SubmitQuestionnaireCommand(Guid RegistrationId, SubmitQuestionnaireRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record UploadQuestionnaireFileCommand(Guid RegistrationId, Guid QuestionId, string ResumeToken, FileItem File) : IRequest<UploadQuestionnaireFileResponse>;
public sealed record DownloadQuestionnaireFileQuery(Guid RegistrationId, Guid FileId, string ResumeToken) : IRequest<QuestionnaireFileContent>;
public sealed record DeleteQuestionnaireFileCommand(Guid RegistrationId, Guid FileId, string ResumeToken) : IRequest;
public sealed record UploadVendorRegistrationDocumentCommand(Guid RegistrationId, string DefinitionKey, string ResumeToken, FileItem File) : IRequest<UploadVendorRegistrationDocumentResponse>;
public sealed record DownloadVendorRegistrationDocumentQuery(Guid RegistrationId, Guid DocumentId, string ResumeToken) : IRequest<QuestionnaireFileContent>;
public sealed record DeleteVendorRegistrationDocumentCommand(Guid RegistrationId, Guid DocumentId, string ResumeToken) : IRequest;
public sealed record QuestionnaireFileContent(byte[] Content, string ContentType, string FileName);

#endregion

#region Validation

public sealed class StartQuestionnaireCommandValidator : AbstractValidatorBase<StartQuestionnaireCommand>
{
    public StartQuestionnaireCommandValidator()
    {
        _ = RuleFor(x => x.Request).SetValidator(new PreRegistrationRequestValidator());
    }
}
public sealed class UpdateVendorRegistrationProfileCommandValidator : AbstractValidatorBase<UpdateVendorRegistrationProfileCommand>
{
    public UpdateVendorRegistrationProfileCommandValidator()
    {
        _ = RuleFor(x => x.Request.Profile).SetValidator(new PreRegistrationRequestValidator());
    }
}

#endregion

#region Questionnaire Lifecycle Handlers

public sealed class StartQuestionnaireHandler(IDatabaseService db, ICurrentUserService currentUser) : IRequestHandler<StartQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(StartQuestionnaireCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var normalizedSap = request.SapVendorNumber.Trim().ToUpperInvariant();
        var userId = currentUser.Username is null
            ? null
            : await db.Users.Where(x => !x.IsDeleted && x.Username == currentUser.Username).Select(x => (Guid?)x.Id).SingleOrDefaultAsync(cancellationToken);
        if (userId is not null && await db.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.UserId == userId, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this account.");
        }

        if (normalizedSap is not null && await db.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.NormalizedSapVendorNumber == normalizedSap, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this SAP vendor number.");
        }

        var questionnaire = await db.Questionnaires.Include(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options)
            .Include(x => x.Rules).SingleOrDefaultAsync(x => !x.IsDeleted && x.Status == QuestionnaireStatus.Publish && x.IsActive && x.Code == "VENDOR_REGISTRATION", cancellationToken)
            ?? throw new ValidationException("No published vendor registration questionnaire is available. Registration cannot be started.");
        var documentRequirementSet = await db.DocumentRequirementSets.SingleOrDefaultAsync(x => !x.IsDeleted && x.Status == QuestionnaireStatus.Publish && x.BusinessProcess == "Vendor Registration", cancellationToken)
            ?? throw new ValidationException("No published vendor registration document requirement set is available. Registration cannot be started.");
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var registration = new VendorRegistration
        {
            UserId = userId,
            CompanyType = request.CompanyStatus!.Value,
            SapVendorNumber = request.SapVendorNumber.Trim(),
            NormalizedSapVendorNumber = normalizedSap,
            CompanyName = request.CompanyName.Trim(),
            CompanyEmail = request.CompanyEmail.Trim(),
            PicEmail = request.PicEmail.Trim(),
            CompanyPhoneNumber = request.CompanyPhoneNumber.Trim(),
            PicPhoneNumber = request.PicPhoneNumber.Trim(),
            Website = request.Website.Trim(),
            CompanyService = request.CompanyService!.Value,
            FactoryCountry = request.FactoryCountry.Trim(),
            FactoryAddress = request.FactoryAddress.Trim(),
            BrandRepresentative = request.BrandRepresentative.Trim(),
            AdditionalBrandsJson = JsonSerializer.Serialize(request.AdditionalBrands),
            IsRepresentativeInIndonesia = request.IsRepresentativeInIndonesia!.Value,
            RepresentativeName = request.RepresentativeName.Trim(),
            ResumeTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(token)),
            ResumeTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(30),
            Status = VendorRegistrationStatus.Draft,
            QuestionnaireId = questionnaire.Id,
            DocumentRequirementSetId = documentRequirementSet.Id
        };
        _ = await db.VendorRegistrations.AddAsync(registration, cancellationToken);
        _ = await db.SaveAsync(nameof(StartQuestionnaireCommand), cancellationToken);
        return await QuestionnaireRuntime.LoadResponse(db, registration.Id, token, token, cancellationToken);
    }
}

public sealed class UpdateVendorRegistrationProfileHandler(IDatabaseService db) : IRequestHandler<UpdateVendorRegistrationProfileCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(UpdateVendorRegistrationProfileCommand command, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(db, command.RegistrationId, command.Request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        QuestionnaireRuntime.SetConcurrency(db, registration, command.Request.RowVersion);
        if (command.Request.Profile.CompanyStatus != registration.CompanyType)
        {
            throw new ValidationException("Company type cannot be changed after registration starts.");
        }

        var normalizedSap = command.Request.Profile.SapVendorNumber.Trim().ToUpperInvariant();
        if (await db.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.Id != registration.Id && x.NormalizedSapVendorNumber == normalizedSap, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this SAP vendor number.");
        }

        QuestionnaireRuntime.AssignProfile(registration, command.Request.Profile);
        _ = await db.SaveAsync(nameof(UpdateVendorRegistrationProfileCommand), cancellationToken);
        return await QuestionnaireRuntime.LoadResponse(db, registration.Id, command.Request.ResumeToken, null, cancellationToken);
    }
}

public sealed class GetQuestionnaireRuntimeHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireRuntimeQuery, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(GetQuestionnaireRuntimeQuery q, CancellationToken cancellationToken)
    {
        return QuestionnaireRuntime.LoadResponse(db, q.RegistrationId, q.ResumeToken, null, cancellationToken);
    }
}

public sealed class SaveQuestionnaireAnswersHandler(IDatabaseService db) : IRequestHandler<SaveQuestionnaireAnswersCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(SaveQuestionnaireAnswersCommand command, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var registration = await QuestionnaireRuntime.Load(db, command.RegistrationId, command.Request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        QuestionnaireRuntime.SetConcurrency(db, registration, command.Request.RowVersion);
        if (command.Request.Answers.GroupBy(x => x.QuestionId).Any(x => x.Count() > 1))
        {
            throw new ValidationException("Duplicate answers are not allowed.");
        }

        var questions = QuestionnaireRuntime.ApplicableQuestions(registration, command.Request.Answers).ToDictionary(x => x.Id);
        var answers = new List<QuestionnaireAnswer>();
        foreach (var value in command.Request.Answers)
        {
            if (!questions.TryGetValue(value.QuestionId, out var question))
            {
                throw new ValidationException("An answer references an unknown or non-applicable question.");
            }

            QuestionnaireRuntime.ValidateValue(question, value);
            var answer = new QuestionnaireAnswer { VendorRegistrationId = registration.Id, QuestionnaireQuestionId = value.QuestionId };
            QuestionnaireRuntime.Assign(answer, value);
            answers.Add(answer);
        }

        var questionIds = command.Request.Answers.Select(x => x.QuestionId).ToList();
        _ = await db.QuestionnaireAnswers
            .Where(x => x.VendorRegistrationId == registration.Id && questionIds.Contains(x.QuestionnaireQuestionId))
            .ExecuteDeleteAsync(cancellationToken);
        await db.QuestionnaireAnswers.AddRangeAsync(answers, cancellationToken);
        _ = await db.SaveAsync(nameof(SaveQuestionnaireAnswersCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return await QuestionnaireRuntime.LoadResponse(db, registration.Id, command.Request.ResumeToken, null, cancellationToken);
    }
}

public sealed class SubmitQuestionnaireHandler(IDatabaseService db) : IRequestHandler<SubmitQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(SubmitQuestionnaireCommand command, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(db, command.RegistrationId, command.Request.ResumeToken, true, cancellationToken);
        if (registration.Status == VendorRegistrationStatus.Submitted)
        {
            return await QuestionnaireRuntime.LoadResponse(db, registration.Id, command.Request.ResumeToken, null, cancellationToken);
        }

        QuestionnaireRuntime.SetConcurrency(db, registration, command.Request.RowVersion);
        if (!await QuestionnaireRuntime.IsDocumentEvidenceComplete(db, registration, cancellationToken))
        {
            throw new ValidationException("All mandatory documents must be uploaded before submission.");
        }

        var applicable = QuestionnaireRuntime.ApplicableQuestions(registration, []).ToList();
        foreach (var question in applicable)
        {
            var answer = registration.Answers.SingleOrDefault(x => x.QuestionnaireQuestionId == question.Id);
            var value = answer is null ? null : QuestionnaireRuntime.ToValue(answer);
            if (QuestionnaireRuntime.IsRequired(registration, question) && !QuestionnaireRuntime.HasValue(value, answer))
            {
                throw new ValidationException($"'{question.Label}' is required.");
            }

            if (value is not null)
            {
                QuestionnaireRuntime.ValidateValue(question, value);
            }
        }

        var ids = applicable.Select(x => x.Id).ToHashSet();
        db.QuestionnaireAnswers.RemoveRange(registration.Answers.Where(x => !ids.Contains(x.QuestionnaireQuestionId) && x.Files.Count == 0));
        registration.Status = VendorRegistrationStatus.Submitted;
        registration.SubmittedAt = DateTimeOffset.UtcNow;
        _ = await db.SaveAsync(nameof(SubmitQuestionnaireCommand), cancellationToken);
        return await QuestionnaireRuntime.LoadResponse(db, registration.Id, command.Request.ResumeToken, null, cancellationToken);
    }
}

#endregion

#region Questionnaire File Handlers

public sealed class UploadQuestionnaireFileHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<UploadQuestionnaireFileCommand, UploadQuestionnaireFileResponse>
{
    public async Task<UploadQuestionnaireFileResponse> Handle(UploadQuestionnaireFileCommand c, CancellationToken cancellationToken)
    {
        var r = await QuestionnaireRuntime.Load(db, c.RegistrationId, c.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(r);
        db.SetQuestionnaireRuntimeGraphUnchanged();
        var q = QuestionnaireRuntime.ApplicableQuestions(r, []).SingleOrDefault(x => x.Id == c.QuestionId && x.Type == QuestionnaireQuestionType.File) ?? throw new ValidationException("The file question is unknown or non-applicable.");
        QuestionnaireRuntime.ValidatePdf(c.File);

        var answer = r.Answers.SingleOrDefault(x => x.QuestionnaireQuestionId == q.Id);
        if (answer is null)
        {
            answer = new QuestionnaireAnswer { VendorRegistrationId = r.Id, QuestionnaireQuestionId = q.Id };
            r.Answers.Add(answer);
        }

        var stored = await storage.CreateAsync(c.File, cancellationToken);
        var existingFiles = answer.Files.Where(x => !x.IsDeleted).ToList();
        foreach (var existingFile in existingFiles)
        {
            existingFile.IsDeleted = true;
        }

        var file = new QuestionnaireAnswerFile { VendorRegistrationId = r.Id, QuestionnaireQuestionId = q.Id, FileStorageId = stored.Id, OriginalFileName = Path.GetFileName(c.File.FileName), ContentType = "application/pdf", Length = c.File.FileContent.LongLength };
        answer.Files.Add(file);
        _ = await db.SaveAsync(nameof(UploadQuestionnaireFileCommand), cancellationToken);
        foreach (var existingFile in existingFiles)
        {
            await storage.DeleteAsync(existingFile.FileStorageId, cancellationToken);
        }

        return new(new(file.Id, file.OriginalFileName, file.ContentType, file.Length), Convert.ToBase64String(r.RowVersion));
    }
}
public sealed class DownloadQuestionnaireFileHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<DownloadQuestionnaireFileQuery, QuestionnaireFileContent>
{
    public async Task<QuestionnaireFileContent> Handle(DownloadQuestionnaireFileQuery q, CancellationToken cancellationToken)
    {
        _ = await QuestionnaireRuntime.Load(db, q.RegistrationId, q.ResumeToken, false, cancellationToken);
        var file = await db.QuestionnaireAnswerFiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == q.FileId && x.VendorRegistrationId == q.RegistrationId && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("File was not found.");
        return new(await storage.ReadAsync(file.FileStorageId, cancellationToken), file.ContentType, file.OriginalFileName);
    }
}
public sealed class DeleteQuestionnaireFileHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<DeleteQuestionnaireFileCommand>
{
    public async Task Handle(DeleteQuestionnaireFileCommand c, CancellationToken cancellationToken)
    {
        var r = await QuestionnaireRuntime.Load(db, c.RegistrationId, c.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(r);
        db.SetQuestionnaireRuntimeGraphUnchanged();
        var file = await db.QuestionnaireAnswerFiles.SingleOrDefaultAsync(x => x.Id == c.FileId && x.VendorRegistrationId == r.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("File was not found.");
        file.IsDeleted = true;
        await storage.DeleteAsync(file.FileStorageId, cancellationToken);
        _ = await db.SaveAsync(nameof(DeleteQuestionnaireFileCommand), cancellationToken);
    }
}

#endregion

#region Registration Document Handlers

public sealed class UploadVendorRegistrationDocumentHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<UploadVendorRegistrationDocumentCommand, UploadVendorRegistrationDocumentResponse>
{
    public async Task<UploadVendorRegistrationDocumentResponse> Handle(UploadVendorRegistrationDocumentCommand command, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(db, command.RegistrationId, command.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        db.SetQuestionnaireRuntimeGraphUnchanged();
        var definition = await db.DocumentDefinitions.SingleOrDefaultAsync(x => !x.IsDeleted && x.IsActive && x.DocumentRequirementSetId == registration.DocumentRequirementSetId && x.Code == command.DefinitionKey, cancellationToken) ?? throw new ValidationException("The document definition is invalid.");
        QuestionnaireRuntime.ValidatePdf(command.File, definition.MaxSizeMb);
        var stored = await storage.CreateAsync(command.File, cancellationToken);
        var current = registration.Documents.SingleOrDefault(x => !x.IsDeleted && x.DocumentDefinitionId == definition.Id);
        if (current is { } existingDocument)
        {
            existingDocument.IsDeleted = true;
        }

        var document = new VendorRegistrationDocument { VendorRegistrationId = registration.Id, DocumentDefinitionId = definition.Id, DefinitionKey = definition.Code, Name = definition.Name, Order = definition.Order, MaxSizeMb = definition.MaxSizeMb, IsMandatory = definition.IsMandatory, FileStorageId = stored.Id, OriginalFileName = Path.GetFileName(command.File.FileName), ContentType = "application/pdf", Length = command.File.FileContent.LongLength };
        registration.Documents.Add(document);
        _ = await db.SaveAsync(nameof(UploadVendorRegistrationDocumentCommand), cancellationToken);
        if (current is not null)
        {
            await storage.DeleteAsync(current.FileStorageId, cancellationToken);
        }

        return new(new(definition.Id, definition.Code, definition.Name, definition.Order, definition.IsMandatory, definition.MaxSizeMb, document.Id, document.OriginalFileName, document.ContentType, document.Length), Convert.ToBase64String(registration.RowVersion), await QuestionnaireRuntime.IsDocumentEvidenceComplete(db, registration, cancellationToken));
    }
}
public sealed class DownloadVendorRegistrationDocumentHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<DownloadVendorRegistrationDocumentQuery, QuestionnaireFileContent>
{
    public async Task<QuestionnaireFileContent> Handle(DownloadVendorRegistrationDocumentQuery query, CancellationToken cancellationToken)
    {
        _ = await QuestionnaireRuntime.Load(db, query.RegistrationId, query.ResumeToken, false, cancellationToken);
        var document = await db.VendorRegistrationDocuments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == query.DocumentId && x.VendorRegistrationId == query.RegistrationId && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Document was not found.");
        return new(await storage.ReadAsync(document.FileStorageId, cancellationToken), document.ContentType, document.OriginalFileName);
    }
}
public sealed class DeleteVendorRegistrationDocumentHandler(IDatabaseService db, IFileStorageDbService storage) : IRequestHandler<DeleteVendorRegistrationDocumentCommand>
{
    public async Task Handle(DeleteVendorRegistrationDocumentCommand command, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(db, command.RegistrationId, command.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        db.SetQuestionnaireRuntimeGraphUnchanged();
        var document = registration.Documents.SingleOrDefault(x => x.Id == command.DocumentId && !x.IsDeleted) ?? throw new InvalidOperationException("Document was not found.");
        document.IsDeleted = true;
        await storage.DeleteAsync(document.FileStorageId, cancellationToken);
        _ = await db.SaveAsync(nameof(DeleteVendorRegistrationDocumentCommand), cancellationToken);
    }
}

#endregion

internal static class QuestionnaireRuntime
{
    #region Registration Lifecycle

    public static async Task<VendorRegistration> Load(IDatabaseService db, Guid id, string token, bool tracking, CancellationToken ct)
    {
        var query = db.VendorRegistrations.Include(x => x.Questionnaire).ThenInclude(x => x!.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options)
            .Include(x => x.Questionnaire).ThenInclude(x => x!.Rules).Include(x => x.Answers).ThenInclude(x => x.Files).Include(x => x.Documents).Where(x => x.Id == id && !x.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        var r = await query.SingleOrDefaultAsync(ct) ?? throw new InvalidOperationException("Registration was not found.");
        if (r.Questionnaire is null)
        {
            throw new InvalidOperationException("This registration is not linked to a questionnaire version. Historical registrations must be repaired explicitly before they can be opened.");
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token ?? string.Empty));
        if (!CryptographicOperations.FixedTimeEquals(hash, r.ResumeTokenHash) || (r.ResumeTokenExpiresAt is not null && r.ResumeTokenExpiresAt <= DateTimeOffset.UtcNow))
        {
            throw new UnauthorizedAccessException("The resume token is invalid or expired.");
        }

        return r;
    }
    public static void EnsureDraft(VendorRegistration r)
    {
        if (r.Status != VendorRegistrationStatus.Draft)
        {
            throw new InvalidOperationException("Submitted registrations cannot be changed.");
        }
    }
    public static void SetConcurrency(IDatabaseService db, VendorRegistration r, string rowVersion)
    {
        try
        {
            db.SetVendorRegistrationOriginalRowVersion(r, Convert.FromBase64String(rowVersion));
        }
        catch (FormatException)
        {
            throw new ValidationException("The row version is invalid.");
        }
    }
    public static void AssignProfile(VendorRegistration registration, PreRegistrationRequest profile)
    {
        registration.SapVendorNumber = profile.SapVendorNumber.Trim();
        registration.NormalizedSapVendorNumber = profile.SapVendorNumber.Trim().ToUpperInvariant();
        registration.CompanyName = profile.CompanyName.Trim();
        registration.CompanyEmail = profile.CompanyEmail.Trim();
        registration.PicEmail = profile.PicEmail.Trim();
        registration.CompanyPhoneNumber = profile.CompanyPhoneNumber.Trim();
        registration.PicPhoneNumber = profile.PicPhoneNumber.Trim();
        registration.Website = profile.Website.Trim();
        registration.CompanyService = profile.CompanyService!.Value;
        registration.FactoryCountry = profile.FactoryCountry.Trim();
        registration.FactoryAddress = profile.FactoryAddress.Trim();
        registration.BrandRepresentative = profile.BrandRepresentative.Trim();
        registration.AdditionalBrandsJson = JsonSerializer.Serialize(profile.AdditionalBrands);
        registration.CompanyType = profile.CompanyStatus!.Value;
        registration.IsRepresentativeInIndonesia = profile.IsRepresentativeInIndonesia!.Value;
        registration.RepresentativeName = profile.RepresentativeName.Trim();
    }
    public static PreRegistrationRequest MapProfile(VendorRegistration r)
    {
        return new() { SapVendorNumber = r.SapVendorNumber ?? string.Empty, CompanyName = r.CompanyName, CompanyEmail = r.CompanyEmail, PicEmail = r.PicEmail, CompanyPhoneNumber = r.CompanyPhoneNumber, PicPhoneNumber = r.PicPhoneNumber, Website = r.Website, CompanyService = r.CompanyService, FactoryCountry = r.FactoryCountry, FactoryAddress = r.FactoryAddress, BrandRepresentative = r.BrandRepresentative, AdditionalBrands = JsonSerializer.Deserialize<List<string>>(r.AdditionalBrandsJson) ?? [], CompanyStatus = r.CompanyType, IsRepresentativeInIndonesia = r.IsRepresentativeInIndonesia, RepresentativeName = r.RepresentativeName };
    }

    #endregion

    #region Document Validation

    public static async Task<bool> IsDocumentEvidenceComplete(IDatabaseService db, VendorRegistration registration, CancellationToken cancellationToken)
    {
        var mandatoryDefinitionIds = await db.DocumentDefinitions.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive && x.IsMandatory && x.DocumentRequirementSetId == registration.DocumentRequirementSetId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var uploadedDefinitionIds = registration.Documents
            .Where(x => !x.IsDeleted)
            .Select(x => x.DocumentDefinitionId)
            .ToHashSet();
        return mandatoryDefinitionIds.All(uploadedDefinitionIds.Contains);
    }
    public static void ValidatePdf(FileItem file, int maxSizeMb = 50)
    {
        if (file.FileContent.LongLength > maxSizeMb * 1024L * 1024L)
        {
            throw new ValidationException($"PDF files cannot exceed {maxSizeMb} MB.");
        }

        if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) || file.FileContent.Length < 5 || !file.FileContent.AsSpan(0, 5).SequenceEqual("%PDF-"u8))
        {
            throw new ValidationException("Only valid PDF files are accepted.");
        }
    }

    #endregion

    #region Question Rules and Values

    public static IEnumerable<QuestionnaireQuestion> ApplicableQuestions(VendorRegistration r, IReadOnlyList<QuestionnaireAnswerValue> incoming)
    {
        var questions = r.Questionnaire!.Sections
            .Where(section => !section.IsDeleted
                && section.IsActive
                && (section.CompanyType is null || section.CompanyType == r.CompanyType))
            .SelectMany(section => section.Questions)
            .Where(question => !question.IsDeleted
                && question.IsActive
                && (question.CompanyType is null || question.CompanyType == r.CompanyType))
            .ToList();
        var visible = questions.Where(x => x.IsVisible).Select(x => x.Id).ToHashSet();
        var values = r.Answers.Select(ToValue).Concat(incoming).GroupBy(x => x.QuestionId).ToDictionary(x => x.Key, x => x.Last());
        foreach (var rule in r.Questionnaire!.Rules.Where(x => !x.IsDeleted && RuleMatches(r, x, values)))
        {
            if (rule.Action == QuestionnaireRuleAction.Hide)
            {
                _ = visible.Remove(rule.TargetQuestionId);
            }
            else if (rule.Action == QuestionnaireRuleAction.Show && questions.Any(x => x.Id == rule.TargetQuestionId))
            {
                _ = visible.Add(rule.TargetQuestionId);
            }
        }

        return questions.Where(x => visible.Contains(x.Id));
    }
    public static bool IsRequired(VendorRegistration r, QuestionnaireQuestion q)
    {
        var values = r.Answers.Select(ToValue).ToDictionary(x => x.QuestionId);
        return q.IsRequired || q.AnswerRule == QuestionnaireAnswerRule.Mandatory || r.Questionnaire!.Rules.Any(x => !x.IsDeleted && x.TargetQuestionId == q.Id && x.Action == QuestionnaireRuleAction.Require && RuleMatches(r, x, values));
    }
    private static bool RuleMatches(VendorRegistration registration, QuestionnaireRule rule, IReadOnlyDictionary<Guid, QuestionnaireAnswerValue> values)
    {
        if (!values.TryGetValue(rule.SourceQuestionId, out var value))
        {
            return false;
        }

        var optionCodes = registration.Questionnaire!.Sections.SelectMany(x => x.Questions).SelectMany(x => x.Options)
            .Where(x => value.OptionIds?.Contains(x.Id) == true).Select(x => x.Code);
        var actual = value.TextValue ?? value.IntegerValue?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? value.DecimalValue?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? value.DateValue?.ToString("O") ?? value.BooleanValue?.ToString() ?? value.AddressJson ?? string.Join(',', optionCodes);
        var comparison = string.Compare(actual, rule.Value, StringComparison.OrdinalIgnoreCase);
        if (decimal.TryParse(actual, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var actualNumber) && decimal.TryParse(rule.Value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var ruleNumber))
        {
            comparison = actualNumber.CompareTo(ruleNumber);
        }

        return rule.Operator switch { QuestionnaireRuleOperator.Equals => comparison == 0, QuestionnaireRuleOperator.NotEquals => comparison != 0, QuestionnaireRuleOperator.Contains => actual.Contains(rule.Value, StringComparison.OrdinalIgnoreCase), QuestionnaireRuleOperator.GreaterThan => comparison > 0, QuestionnaireRuleOperator.LessThan => comparison < 0, _ => false };
    }
    public static void ValidateValue(QuestionnaireQuestion q, QuestionnaireAnswerValue v)
    {
        var scalarCount = new object?[] { v.TextValue, v.IntegerValue, v.DecimalValue, v.DateValue, v.BooleanValue, v.AddressJson }.Count(x => x is not null);
        var optionIds = v.OptionIds ?? [];
        if (optionIds.Count != optionIds.Distinct().Count())
        {
            throw new ValidationException("Duplicate selected options are not allowed.");
        }

        var expectedScalar = q.Type switch { QuestionnaireQuestionType.ShortText or QuestionnaireQuestionType.LongText => v.TextValue is not null, QuestionnaireQuestionType.Integer => v.IntegerValue is not null, QuestionnaireQuestionType.Decimal => v.DecimalValue is not null, QuestionnaireQuestionType.Date => v.DateValue is not null, QuestionnaireQuestionType.Boolean => v.BooleanValue is not null, QuestionnaireQuestionType.Address => v.AddressJson is not null, QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice or QuestionnaireQuestionType.File => false, _ => false };
        if (q.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice)
        {
            if (scalarCount != 0 || (q.Type == QuestionnaireQuestionType.SingleChoice && optionIds.Count > 1) || optionIds.Any(id => q.Options.All(o => o.Id != id)))
            {
                throw new ValidationException($"The value for '{q.Label}' is invalid.");
            }
        }
        else if (q.Type == QuestionnaireQuestionType.File)
        {
            if (scalarCount != 0 || optionIds.Count != 0)
            {
                throw new ValidationException($"The value for '{q.Label}' is invalid.");
            }
        }
        else if (optionIds.Count != 0 || scalarCount > 1 || (scalarCount == 1 && !expectedScalar))
        {
            throw new ValidationException($"The value for '{q.Label}' has the wrong type.");
        }

        if (v.AddressJson is not null)
        {
            try
            {
                using var document = JsonDocument.Parse(v.AddressJson);
                if (document.RootElement.ValueKind != JsonValueKind.Object || JsonSerializer.Deserialize<QuestionnaireAddressRequest>(v.AddressJson) is null)
                {
                    throw new ValidationException("The address value must be a JSON object.");
                }
            }
            catch (JsonException)
            {
                throw new ValidationException("The address value must be valid JSON.");
            }
        }
    }
    public static void Assign(QuestionnaireAnswer a, QuestionnaireAnswerValue v)
    {
        a.TextValue = v.TextValue;
        a.IntegerValue = v.IntegerValue;
        a.DecimalValue = v.DecimalValue;
        a.DateValue = v.DateValue;
        a.BooleanValue = v.BooleanValue;
        a.JsonValue = v.AddressJson;
        a.JsonValue = v.OptionIds is null ? a.JsonValue : JsonSerializer.Serialize(v.OptionIds);
    }
    public static QuestionnaireAnswerValue ToValue(QuestionnaireAnswer a)
    {
        IReadOnlyList<Guid>? optionIds = null;
        var addressJson = a.JsonValue;
        if (!string.IsNullOrWhiteSpace(a.JsonValue))
        {
            try
            {
                using var document = JsonDocument.Parse(a.JsonValue);
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    optionIds = document.RootElement.EnumerateArray()
                        .Where(item => item.ValueKind == JsonValueKind.String && item.TryGetGuid(out _))
                        .Select(item => item.GetGuid())
                        .ToList();
                    addressJson = null;
                }
            }
            catch (JsonException)
            {
                optionIds = null;
            }
        }

        return new(a.QuestionnaireQuestionId, a.TextValue, a.IntegerValue, a.DecimalValue, a.DateValue, a.BooleanValue, addressJson, optionIds);
    }

    public static bool HasValue(QuestionnaireAnswerValue? v, QuestionnaireAnswer? a)
    {
        return v is not null && ((v.TextValue is not null && !string.IsNullOrWhiteSpace(v.TextValue)) || v.IntegerValue is not null || v.DecimalValue is not null || v.DateValue is not null || v.BooleanValue is not null || HasCompleteAddress(v.AddressJson) || v.OptionIds?.Count > 0 || a?.Files.Any(x => !x.IsDeleted) == true);
    }

    private static bool HasCompleteAddress(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var address = JsonSerializer.Deserialize<QuestionnaireAddressRequest>(json);
            return address is not null
                && !string.IsNullOrWhiteSpace(address.Country)
                && !string.IsNullOrWhiteSpace(address.Building)
                && !string.IsNullOrWhiteSpace(address.Street)
                && !string.IsNullOrWhiteSpace(address.Number)
                && !string.IsNullOrWhiteSpace(address.City)
                && !string.IsNullOrWhiteSpace(address.Phone)
                && !string.IsNullOrWhiteSpace(address.Fax)
                && !string.IsNullOrWhiteSpace(address.Email)
                && !string.IsNullOrWhiteSpace(address.Website);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    #endregion

    #region Response Mapping

    public static async Task<QuestionnaireRuntimeResponse> LoadResponse(IDatabaseService db, Guid id, string authToken, string? returnedToken, CancellationToken ct)
    {
        var r = await Load(db, id, authToken, false, ct);
        var applicable = ApplicableQuestions(r, []).ToHashSet();
        var sections = r.Questionnaire!.Sections.Where(s => !s.IsDeleted).OrderBy(s => s.Order).Select(s => new RuntimeSectionItem(s.Id, s.Code, s.Title, s.Order, s.Questions.Where(applicable.Contains).OrderBy(q => q.Order).Select(q =>
        {
            var a = r.Answers.SingleOrDefault(x => x.QuestionnaireQuestionId == q.Id);
            return new RuntimeQuestionItem(q.Id, q.Code, q.Label, q.Hint, q.Placeholder, q.Type, q.Order, IsRequired(r, q), q.Options.Where(o => !o.IsDeleted).OrderBy(o => o.Order).Select(o => new RuntimeOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList(), a is null ? null : ToValue(a), a?.Files.Where(f => !f.IsDeleted).Select(f => new RuntimeFileItem(f.Id, f.OriginalFileName, f.ContentType, f.Length)).ToList() ?? []);
        }).ToList())).Where(s => s.Questions.Count != 0).ToList();
        var definitions = await db.DocumentDefinitions.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive && x.DocumentRequirementSetId == r.DocumentRequirementSetId)
            .OrderBy(x => x.Order).ThenBy(x => x.Code)
            .ToListAsync(ct);
        var uploaded = r.Documents.Where(x => !x.IsDeleted).ToDictionary(x => x.DocumentDefinitionId);
        var documents = definitions.Select(definition =>
        {
            _ = uploaded.TryGetValue(definition.Id, out var file);
            return new VendorRegistrationDocumentItem(definition.Id, definition.Code, definition.Name, definition.Order, definition.IsMandatory, definition.MaxSizeMb, file?.Id, file?.OriginalFileName, file?.ContentType, file?.Length);
        }).ToList();
        return new(r.Id, returnedToken, Convert.ToBase64String(r.RowVersion), r.Status, MapProfile(r), documents, await IsDocumentEvidenceComplete(db, r, ct), r.QuestionnaireId!.Value, sections);
    }

    #endregion
}
