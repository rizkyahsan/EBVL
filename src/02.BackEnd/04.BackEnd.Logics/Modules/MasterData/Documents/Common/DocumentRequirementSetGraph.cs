using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;

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
