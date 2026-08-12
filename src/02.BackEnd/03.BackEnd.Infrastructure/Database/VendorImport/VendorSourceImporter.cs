using Microsoft.Data.SqlClient;

namespace EBVL.BackEnd.Infrastructure.Database.VendorImport;

public sealed class VendorSourceImporter(
    IDatabaseService target,
    ILogger<VendorSourceImporter> logger)
{
    public async Task<ImportResult> ImportAsync(string sourceConnectionString, CancellationToken cancellationToken)
    {
        var runId = Guid.NewGuid();
        await using var source = new SqlConnection(sourceConnectionString);
        await source.OpenAsync(cancellationToken);
        var result = new ImportResult(runId, target);
        _ = target.VendorMigrationRuns.Add(new VendorMigrationRun
        {
            Id = runId,
            SourceDatabase = source.Database,
            Started = DateTimeOffset.UtcNow,
            Status = "Running"
        });

        await ImportVendorTypesAsync(source, target, result, cancellationToken);
        await ImportContactTypesAsync(source, target, result, cancellationToken);
        await ImportTemplatesAsync(source, target, result, cancellationToken);
        await ImportVendorsAsync(source, target, result, cancellationToken);
        await ImportDocumentsAsync(source, target, result, cancellationToken);
        result.Complete();
        _ = await target.SaveAsync(nameof(VendorSourceImporter), cancellationToken);
        logger.LogInformation("Vendor source import completed. Imported {Imported}, quarantined {Quarantined}.", result.Imported, result.Quarantined);
        return result;
    }

    private static async Task ImportVendorTypesAsync(SqlConnection source, IDatabaseService target, ImportResult result, CancellationToken ct)
    {
        await using var command = new SqlCommand("SELECT Id, VendorTypeName, Description FROM dbo.VendorTypes", source);
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetGuid(0);
            if (!await target.VendorTypes.AnyAsync(x => x.Id == id, ct))
            {
                _ = await target.VendorTypes.AddAsync(new VendorType { Id = id, Name = Limit(reader.IsDBNull(1) ? null : reader.GetString(1), 100) ?? "UNKNOWN", Description = Limit(reader.IsDBNull(2) ? null : reader.GetString(2), 500) }, ct);
                result.ImportedRow("VendorTypes", id, "VendorTypes", id);
            }
        }
    }

    private static async Task ImportContactTypesAsync(SqlConnection source, IDatabaseService target, ImportResult result, CancellationToken ct)
    {
        await using var command = new SqlCommand("SELECT Id, ContactTypeName, Description FROM dbo.ContactTypes", source);
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetGuid(0);
            if (!await target.ContactTypes.AnyAsync(x => x.Id == id, ct))
            {
                _ = await target.ContactTypes.AddAsync(new ContactType { Id = id, Name = Limit(reader.IsDBNull(1) ? null : reader.GetString(1), 100) ?? "UNKNOWN", Description = Limit(reader.IsDBNull(2) ? null : reader.GetString(2), 500) }, ct);
                result.ImportedRow("ContactTypes", id, "ContactTypes", id);
            }
        }
    }

    private static async Task ImportTemplatesAsync(SqlConnection source, IDatabaseService target, ImportResult result, CancellationToken ct)
    {
        await using var command = new SqlCommand("SELECT Id, DocumentTemplateName, Alias, IsMandatory FROM dbo.DocumentTemplates", source);
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetGuid(0);
            if (!await target.DocumentTemplates.AnyAsync(x => x.Id == id, ct))
            {
                _ = await target.DocumentTemplates.AddAsync(new DocumentTemplate { Id = id, Name = Limit(reader.IsDBNull(1) ? null : reader.GetString(1), 200) ?? "UNKNOWN", Alias = Limit(reader.IsDBNull(2) ? null : reader.GetString(2), 200), IsMandatory = !reader.IsDBNull(3) && reader.GetBoolean(3) }, ct);
                result.ImportedRow("DocumentTemplates", id, "DocumentTemplates", id);
            }
        }
    }

    private static async Task ImportVendorsAsync(SqlConnection source, IDatabaseService target, ImportResult result, CancellationToken ct)
    {
        var duplicateSap = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using (var duplicateCommand = new SqlCommand("SELECT PTMSAPVendorNo FROM dbo.Vendors WHERE PTMSAPVendorNo IS NOT NULL GROUP BY PTMSAPVendorNo HAVING COUNT_BIG(*) > 1", source))
        await using (var duplicateReader = await duplicateCommand.ExecuteReaderAsync(ct))
        {
            while (await duplicateReader.ReadAsync(ct))
            {
                _ = duplicateSap.Add(duplicateReader.GetString(0).Trim());
            }
        }

        await using var command = new SqlCommand("SELECT Id, VendorTypeID, VendorName, VendorMail, NPWP, Website, PTMSAPVendorNo, IsConfimed, IsDeleted FROM dbo.Vendors", source);
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetGuid(0);
            var sap = reader.IsDBNull(6) ? null : reader.GetString(6).Trim();
            if (string.IsNullOrWhiteSpace(sap))
            {
                result.Quarantine("Vendors", id, "MissingSapVendorNumber");
                continue;
            }

            if (duplicateSap.Contains(sap))
            {
                result.Quarantine("Vendors", id, "DuplicateSapVendorNumber");
                continue;
            }

            if (!target.VendorTypes.Local.Any(x => x.Id == reader.GetGuid(1)) && !await target.VendorTypes.AnyAsync(x => x.Id == reader.GetGuid(1), ct))
            {
                result.Quarantine("Vendors", id, "MissingVendorType");
                continue;
            }

            if (target.Vendors.Local.Any(x => x.Id == id || x.SapVendorNumber == sap) || await target.Vendors.AnyAsync(x => x.Id == id || x.SapVendorNumber == sap, ct))
            {
                result.Quarantine("Vendors", id, "TargetConflict");
                continue;
            }

            var email = reader.IsDBNull(3) ? null : reader.GetString(3).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                result.Quarantine("Vendors", id, "MissingEmail");
                continue;
            }

            _ = await target.Vendors.AddAsync(new Vendor
            {
                Id = id,
                SapVendorNumber = Limit(sap, 50)!,
                Name = Limit(reader.IsDBNull(2) ? null : reader.GetString(2), 250) ?? "UNKNOWN",
                Email = Limit(email, 320)!,
                TaxId = Limit(reader.IsDBNull(4) ? null : reader.GetString(4).Trim(), 100),
                Website = Limit(reader.IsDBNull(5) ? null : reader.GetString(5).Trim(), 2048),
                VendorTypeId = reader.GetGuid(1),
                LegacyConfirmedStatus = reader.IsDBNull(7) ? null : reader.GetInt32(7).ToString(System.Globalization.CultureInfo.InvariantCulture),
                IsDeleted = !reader.IsDBNull(8) && reader.GetBoolean(8)
            }, ct);
            result.ImportedRow("Vendors", id, "Vendors", id);
        }
    }

    private static async Task ImportDocumentsAsync(SqlConnection source, IDatabaseService target, ImportResult result, CancellationToken ct)
    {
        await using var command = new SqlCommand("SELECT Id, VendorID, DocumentTemplateID, FileName, FileContentType, FileSize, StorageFileId, ValidUntil FROM dbo.VendorDocuments", source);
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetGuid(0);
            var vendorId = reader.GetGuid(1);
            var templateId = reader.GetGuid(2);
            if (!target.Vendors.Local.Any(x => x.Id == vendorId) && !await target.Vendors.AnyAsync(x => x.Id == vendorId, ct))
            {
                result.Quarantine("VendorDocuments", id, "MissingImportedVendor");
                continue;
            }

            if (!target.DocumentTemplates.Local.Any(x => x.Id == templateId) && !await target.DocumentTemplates.AnyAsync(x => x.Id == templateId, ct))
            {
                result.Quarantine("VendorDocuments", id, "MissingDocumentTemplate");
                continue;
            }

            if (await target.VendorDocuments.AnyAsync(x => x.Id == id, ct))
            {
                continue;
            }

            var original = reader.IsDBNull(3) ? null : Path.GetFileName(reader.GetString(3));
            var storage = reader.IsDBNull(6) ? null : reader.GetString(6).Trim();
            if (string.IsNullOrWhiteSpace(original) || string.IsNullOrWhiteSpace(storage))
            {
                result.Quarantine("VendorDocuments", id, "FileMissing");
                continue;
            }

            _ = await target.VendorDocuments.AddAsync(new VendorDocument
            {
                Id = id,
                VendorId = vendorId,
                DocumentTemplateId = templateId,
                OriginalFileName = Limit(original, 200)!,
                StoredFileName = Limit(original, 200)!,
                FileContentType = Limit(reader.IsDBNull(4) ? null : reader.GetString(4), 100) ?? "application/octet-stream",
                FileSize = reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
                StorageFileId = Limit(storage, 500),
                ValidUntil = reader.IsDBNull(7) ? null : reader.GetDateTimeOffset(7),
                IsVerified = false
            }, ct);
            result.ImportedRow("VendorDocuments", id, "VendorDocuments", id);
        }
    }

    private static string? Limit(string? value, int length)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().Length <= length ? value.Trim() : null;
    }
}

public sealed class ImportResult(Guid runId, IDatabaseService target)
{
    public Guid RunId { get; } = runId;
    public int Imported { get; set; }
    public int Quarantined { get; set; }
    public List<string> Reasons { get; } = [];
    public void ImportedRow(string sourceTable, Guid sourceId, string targetTable, Guid targetId)
    {
        Imported++;
        _ = target.VendorMigrationRows.Add(new VendorMigrationRow { Id = Guid.NewGuid(), RunId = RunId, SourceTable = sourceTable, SourceId = sourceId, Outcome = "Imported", Processed = DateTimeOffset.UtcNow });
        _ = target.VendorMigrationCrosswalks.Add(new VendorMigrationCrosswalk { Id = Guid.NewGuid(), RunId = RunId, SourceTable = sourceTable, SourceId = sourceId, TargetTable = targetTable, TargetId = targetId, Created = DateTimeOffset.UtcNow });
    }

    public void Quarantine(string table, Guid id, string reason)
    {
        Quarantined++;
        Reasons.Add($"{table}:{id:N}:{reason}");
        _ = target.VendorMigrationRows.Add(new VendorMigrationRow { Id = Guid.NewGuid(), RunId = RunId, SourceTable = table, SourceId = id, Outcome = "Quarantined", Reason = reason, Processed = DateTimeOffset.UtcNow });
        _ = target.VendorMigrationQuarantines.Add(new VendorMigrationQuarantine { Id = Guid.NewGuid(), RunId = RunId, SourceTable = table, SourceId = id, Reason = reason, Created = DateTimeOffset.UtcNow });
    }

    public void Complete()
    {
        var run = target.VendorMigrationRuns.Local.Single(x => x.Id == RunId);
        run.Completed = DateTimeOffset.UtcNow;
        run.Status = "Completed";
        run.Imported = Imported;
        run.Quarantined = Quarantined;
    }
}
