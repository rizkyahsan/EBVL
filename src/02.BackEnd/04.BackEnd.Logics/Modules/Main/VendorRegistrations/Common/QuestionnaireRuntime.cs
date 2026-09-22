using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;

public sealed record QuestionnaireFileContent(byte[] Content, string ContentType, string FileName);

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
