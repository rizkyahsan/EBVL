using EBVL.Shared.Dto.Modules.MasterData.Documents;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents;

public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>;
public sealed record GetDocumentQuery(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;
public sealed record GetDocumentHistoryQuery(Guid SeriesId) : IRequest<GetDocumentHistoryResponse>;
public sealed record CreateDocumentDraftCommand(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;
public sealed record PublishDocumentRequirementSetCommand(Guid DocumentRequirementSetId, string RowVersion) : IRequest<GetDocumentResponse>;
public sealed record AddDocumentCommand(Guid DocumentRequirementSetId, AddDocumentRequest Document) : IRequest<GetDocumentResponse>;
public sealed record UpdateDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, UpdateDocumentRequest Document) : IRequest<GetDocumentResponse>;
public sealed record DeleteDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, string RowVersion) : IRequest<GetDocumentResponse>;

public sealed class AddDocumentCommandValidator : AbstractValidatorBase<AddDocumentCommand> { public AddDocumentCommandValidator() { _ = RuleFor(x => x.Document).SetValidator(new AddDocumentRequestValidator()); } }
public sealed class UpdateDocumentCommandValidator : AbstractValidatorBase<UpdateDocumentCommand> { public UpdateDocumentCommandValidator() { _ = RuleFor(x => x.Document).SetValidator(new UpdateDocumentRequestValidator()); } }

public sealed class GetDocumentsHandler(IDatabaseService db) : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public async Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.DocumentRequirementSets.AsNoTracking().Where(x => !x.IsDeleted)
            .Where(x => !db.DocumentRequirementSets.Any(other => !other.IsDeleted && other.DocumentRequirementSetSeriesId == x.DocumentRequirementSetSeriesId && other.Version > x.Version))
            .OrderBy(x => x.BusinessProcess).Select(x => new DocumentRequirementSetListItem(x.Id, x.DocumentRequirementSetSeriesId, x.BusinessProcess, x.Version, x.Status, x.Modified ?? x.Created, x.Requirements.Count(r => !r.IsDeleted), Convert.ToBase64String(x.RowVersion))).ToListAsync(cancellationToken);
        return new() { Items = items };
    }
}
public sealed class GetDocumentHandler(IDatabaseService db) : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        return new() { Item = DocumentRequirementSetGraph.Map(await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, false, cancellationToken)) };
    }
}
public sealed class GetDocumentHistoryHandler(IDatabaseService db) : IRequestHandler<GetDocumentHistoryQuery, GetDocumentHistoryResponse>
{
    public async Task<GetDocumentHistoryResponse> Handle(GetDocumentHistoryQuery request, CancellationToken cancellationToken)
    {
        return new() { Items = await db.DocumentRequirementSets.AsNoTracking().Where(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == request.SeriesId).OrderByDescending(x => x.Version).Select(x => new DocumentRequirementSetVersionItem(x.Id, x.Version, x.Status, x.PublishedAt, x.PublishedBy, x.Created)).ToListAsync(cancellationToken) };
    }
}
public sealed class CreateDocumentDraftHandler(IDatabaseService db) : IRequestHandler<CreateDocumentDraftCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(CreateDocumentDraftCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var source = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        var existing = await db.DocumentRequirementSets.Include(x => x.Requirements).SingleOrDefaultAsync(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == source.DocumentRequirementSetSeriesId && x.Status == QuestionnaireStatus.Draft, cancellationToken);
        if (existing is not null)
        {
            await tx.CommitAsync(cancellationToken);
            return new() { Item = DocumentRequirementSetGraph.Map(existing) };
        }

        if (source.Status != QuestionnaireStatus.Publish)
        {
            throw new ValidationException("A draft can only be created from the published document requirement set.");
        }

        var version = await db.DocumentRequirementSets.Where(x => x.DocumentRequirementSetSeriesId == source.DocumentRequirementSetSeriesId).MaxAsync(x => x.Version, cancellationToken) + 1;
        var draft = DocumentRequirementSetGraph.Clone(source, version);
        _ = db.DocumentRequirementSets.Add(draft);
        _ = await db.SaveAsync(nameof(CreateDocumentDraftCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(draft) };
    }
}
public sealed class PublishDocumentRequirementSetHandler(IDatabaseService db, ICurrentUserService currentUser) : IRequestHandler<PublishDocumentRequirementSetCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(PublishDocumentRequirementSetCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var draft = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(draft);
        DocumentRequirementSetGraph.EnsureRowVersion(draft, request.RowVersion);
        if (!draft.Requirements.Any(x => !x.IsDeleted && x.IsActive))
        {
            throw new ValidationException("At least one active document requirement is required.");
        }

        var current = await db.DocumentRequirementSets.SingleOrDefaultAsync(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == draft.DocumentRequirementSetSeriesId && x.Status == QuestionnaireStatus.Publish, cancellationToken);
        try
        {
            if (current is { } published)
            {
                published.Status = QuestionnaireStatus.Superseded;
                _ = await db.SaveAsync(nameof(PublishDocumentRequirementSetCommand), cancellationToken);
            }

            draft.Status = QuestionnaireStatus.Publish;
            draft.PublishedAt = DateTimeOffset.UtcNow;
            draft.PublishedBy = currentUser.Username ?? "EBVLSystem";
            _ = await db.SaveAsync(nameof(PublishDocumentRequirementSetCommand), cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ValidationException("The document requirement set changed or was published concurrently. Reload and try again.");
        }

        await tx.CommitAsync(cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(draft) };
    }
}
public sealed class AddDocumentHandler(IDatabaseService db) : IRequestHandler<AddDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.Document.RowVersion);
        DocumentRequirementSetGraph.EnsureProcess(set, request.Document.BusinessProcess);
        var name = request.Document.Name.Trim();
        if (set.Requirements.Any(x => !x.IsDeleted && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A document type with the same name already exists.");
        }

        set.Modified = DateTimeOffset.UtcNow;
        var active = set.Requirements.Where(x => !x.IsDeleted).ToList();
        var requestedOrder = Math.Clamp(request.Document.Order, 1, active.Count + 1);
        foreach (var item in active.Where(x => x.Order >= requestedOrder))
        {
            item.Order++;
        }

        var document = new DocumentDefinition { Id = Guid.CreateVersion7(), DocumentRequirementSetId = set.Id, Code = DocumentRequirementSetGraph.NextCode(set), BusinessProcess = set.BusinessProcess, Name = name, Order = requestedOrder, MaxSizeMb = request.Document.MaxSizeMb, IsMandatory = request.Document.IsMandatory, IsActive = request.Document.IsActive };
        _ = db.DocumentDefinitions.Add(document);
        await DocumentRequirementSetGraph.Save(db, nameof(AddDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}
public sealed class UpdateDocumentHandler(IDatabaseService db) : IRequestHandler<UpdateDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.Document.RowVersion);
        DocumentRequirementSetGraph.EnsureProcess(set, request.Document.BusinessProcess);
        var document = set.Requirements.SingleOrDefault(x => x.Id == request.DocumentId && !x.IsDeleted) ?? throw new KeyNotFoundException("Document requirement was not found.");
        var name = request.Document.Name.Trim();
        if (set.Requirements.Any(x => x.Id != document.Id && !x.IsDeleted && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A document type with the same name already exists.");
        }

        document.Name = name;
        document.MaxSizeMb = request.Document.MaxSizeMb;
        document.IsMandatory = request.Document.IsMandatory;
        document.IsActive = request.Document.IsActive;
        var ordered = set.Requirements.Where(x => !x.IsDeleted && x.Id != document.Id).OrderBy(x => x.Order).ToList();
        ordered.Insert(Math.Clamp(request.Document.Order - 1, 0, ordered.Count), document);
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Order = index + 1;
        }

        set.Modified = DateTimeOffset.UtcNow;
        await DocumentRequirementSetGraph.Save(db, nameof(UpdateDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}
public sealed class DeleteDocumentHandler(IDatabaseService db) : IRequestHandler<DeleteDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.RowVersion);
        var document = set.Requirements.SingleOrDefault(x => x.Id == request.DocumentId && !x.IsDeleted) ?? throw new KeyNotFoundException("Document requirement was not found.");
        _ = db.DocumentDefinitions.Remove(document);
        _ = set.Requirements.Remove(document);
        set.Modified = DateTimeOffset.UtcNow;
        var order = 1;
        foreach (var item in set.Requirements.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        await DocumentRequirementSetGraph.Save(db, nameof(DeleteDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}

internal static class DocumentRequirementSetGraph
{
    public static void EnsureDraft(DocumentRequirementSet set)
    {
        if (set.Status != QuestionnaireStatus.Draft)
        {
            throw new ValidationException("Published and superseded document requirement sets are read-only. Create or open the draft version to edit.");
        }
    }
    public static void EnsureProcess(DocumentRequirementSet set, string process)
    {
        if (!string.Equals(set.BusinessProcess, process.Trim(), StringComparison.Ordinal))
        {
            throw new ValidationException("The business process of a document requirement set cannot be changed.");
        }
    }
    public static void EnsureRowVersion(DocumentRequirementSet set, string value)
    {
        byte[] expected;
        try
        {
            expected = Convert.FromBase64String(value);
        }
        catch (FormatException)
        {
            throw new ValidationException("The row version is invalid.");
        }

        if (!set.RowVersion.SequenceEqual(expected))
        {
            throw new ValidationException("The document requirement set changed. Reload and try again.");
        }
    }
    public static Task<DocumentRequirementSet> Load(IDatabaseService db, Guid id, bool tracking, CancellationToken ct)
    {
        var query = db.DocumentRequirementSets.Include(x => x.Requirements).Where(x => x.Id == id && !x.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(ct).ContinueWith(x => x.Result ?? throw new KeyNotFoundException("Document requirement set was not found."), ct);
    }
    public static DocumentRequirementSet Clone(DocumentRequirementSet source, int version)
    {
        var id = Guid.CreateVersion7();
        var result = new DocumentRequirementSet { Id = id, DocumentRequirementSetSeriesId = source.DocumentRequirementSetSeriesId, BusinessProcess = source.BusinessProcess, Version = version, Status = QuestionnaireStatus.Draft, PreviousVersionId = source.Id };
        foreach (var x in source.Requirements.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            result.Requirements.Add(new DocumentDefinition { Id = Guid.CreateVersion7(), DocumentRequirementSetId = id, Code = x.Code, BusinessProcess = x.BusinessProcess, Name = x.Name, Order = x.Order, MaxSizeMb = x.MaxSizeMb, IsMandatory = x.IsMandatory, IsActive = x.IsActive });
        }

        return result;
    }
    public static string NextCode(DocumentRequirementSet set)
    {
        var prefix = new string([.. set.BusinessProcess.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant)]);
        var sequence = set.Requirements.Count + 1;
        string code;
        do
        {
            code = $"{prefix}_{sequence++:000}";
        }
        while (set.Requirements.Any(x => x.Code == code));
        return code;
    }
    public static DocumentRequirementSetItem Map(DocumentRequirementSet x)
    {
        return new(x.Id, x.DocumentRequirementSetSeriesId, x.BusinessProcess, x.Version, x.Status, x.PreviousVersionId, x.PublishedAt, x.PublishedBy, Convert.ToBase64String(x.RowVersion), x.Requirements.Where(r => !r.IsDeleted).OrderBy(r => r.Order).Select(r => new DocumentListItem(r.Id, x.Id, x.BusinessProcess, r.Name, r.Order, r.MaxSizeMb, r.IsMandatory, r.IsActive, Convert.ToBase64String(x.RowVersion))).ToList());
    }

    public static async Task Save(IDatabaseService db, string action, CancellationToken ct)
    {
        try
        {
            _ = await db.SaveAsync(action, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("The document requirement set changed. Reload and try again.");
        }
    }
}
