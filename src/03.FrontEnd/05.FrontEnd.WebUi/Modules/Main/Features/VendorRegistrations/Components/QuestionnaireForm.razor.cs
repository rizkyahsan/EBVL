using System.Text.Json;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireForm
{
    #region Parameters

    [Parameter] public required IReadOnlyList<RuntimeSectionItem> Sections { get; init; }
    [Parameter] public required EventCallback OnBack { get; init; }
    [Parameter] public required EventCallback<IReadOnlyList<QuestionnaireAnswerValue>> OnNext { get; init; }
    [Parameter] public required Func<RuntimeQuestionItem, IBrowserFile, Task> OnFileSelected { get; init; }
    [Parameter] public required EventCallback<RuntimeFileItem> OnFileRemoved { get; init; }

    #endregion

    #region Fields

    private readonly Dictionary<Guid, string?> _texts = [];
    private readonly Dictionary<Guid, string?> _fileNames = [];
    private readonly Dictionary<Guid, QuestionnaireAddressRequest> _addresses = [];
    private readonly Dictionary<Guid, long?> _integers = [];
    private readonly Dictionary<Guid, decimal?> _decimals = [];
    private readonly Dictionary<Guid, DateOnly?> _dates = [];
    private readonly Dictionary<Guid, bool?> _booleans = [];
    private readonly Dictionary<Guid, HashSet<Guid>> _optionIds = [];
    private string? _validationError;
    private bool _isSubmitting;

    #endregion

    #region Lifecycle

    protected override void OnParametersSet()
    {
        foreach (var question in Sections.SelectMany(section => section.Questions))
        {
            if (!_texts.ContainsKey(question.Id))
            {
                _texts[question.Id] = question.Answer?.TextValue;
            }

            if (!_fileNames.ContainsKey(question.Id))
            {
                _fileNames[question.Id] = question.Files.FirstOrDefault()?.FileName;
            }

            if (question.Type == QuestionnaireQuestionType.Address && !_addresses.ContainsKey(question.Id))
            {
                _addresses[question.Id] = DeserializeAddress(question.Answer?.AddressJson);
            }

            _ = _integers.TryAdd(question.Id, question.Answer?.IntegerValue);
            _ = _decimals.TryAdd(question.Id, question.Answer?.DecimalValue);
            _ = _dates.TryAdd(question.Id, question.Answer?.DateValue);
            _ = _booleans.TryAdd(question.Id, question.Answer?.BooleanValue);
            _ = _optionIds.TryAdd(question.Id, question.Answer?.OptionIds?.ToHashSet() ?? []);
        }
    }

    #endregion

    #region Private Methods

    private string QuestionLabel(RuntimeQuestionItem question)
    {
        return question.Label + (question.IsRequired ? " *" : string.Empty);
    }

    private string? GetText(Guid id)
    {
        return _texts.GetValueOrDefault(id);
    }

    private void SetText(Guid id, string? value)
    {
        _texts[id] = value;
    }

    private QuestionnaireAddressRequest GetAddress(Guid id)
    {
        return _addresses.GetValueOrDefault(id) ?? new();
    }

    private long? GetInteger(Guid id)
    {
        return _integers.GetValueOrDefault(id);
    }

    private void SetInteger(Guid id, long? value)
    {
        _integers[id] = value;
    }

    private decimal? GetDecimal(Guid id)
    {
        return _decimals.GetValueOrDefault(id);
    }

    private void SetDecimal(Guid id, decimal? value)
    {
        _decimals[id] = value;
    }

    private DateTime? GetDate(Guid id)
    {
        return _dates.GetValueOrDefault(id)?.ToDateTime(TimeOnly.MinValue);
    }

    private void SetDate(Guid id, DateTime? value)
    {
        _dates[id] = value is null ? null : DateOnly.FromDateTime(value.Value);
    }

    private bool? GetBoolean(Guid id)
    {
        return _booleans.GetValueOrDefault(id);
    }

    private void SetBoolean(Guid id, bool? value)
    {
        _booleans[id] = value;
    }

    private Guid? GetSingleOption(Guid id)
    {
        return _optionIds.GetValueOrDefault(id)?.SingleOrDefault() is var value && value != Guid.Empty ? value : null;
    }

    private void SetSingleOption(Guid id, Guid? value)
    {
        _optionIds[id] = value is null ? [] : [value.Value];
    }

    private bool IsOptionSelected(Guid questionId, Guid optionId)
    {
        return _optionIds.GetValueOrDefault(questionId)?.Contains(optionId) == true;
    }

    private void SetOptionSelected(Guid questionId, Guid optionId, bool selected)
    {
        var selectedOptions = _optionIds.GetValueOrDefault(questionId) ?? [];
        _optionIds[questionId] = selectedOptions;
        if (selected)
        {
            _ = selectedOptions.Add(optionId);
        }
        else
        {
            _ = selectedOptions.Remove(optionId);
        }
    }

    private static QuestionnaireAddressRequest DeserializeAddress(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new();
        }

        try
        {
            return JsonSerializer.Deserialize<QuestionnaireAddressRequest>(value) ?? new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    private void AddressChanged(Guid _) { }

    private async Task SelectFile(RuntimeQuestionItem question, InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (!string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            _validationError = "Only PDF files are allowed.";
            return;
        }

        if (file.Size > 50L * 1024 * 1024)
        {
            _validationError = "The maximum file size is 50 MB.";
            return;
        }

        _validationError = null;
        _fileNames[question.Id] = file.Name;
        await InvokeAsync(StateHasChanged);
        try
        {
            await OnFileSelected(question, file);
        }
        catch (Exception exception)
        {
            _ = _fileNames.Remove(question.Id);
            _validationError = exception.Message;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task RemoveFile(RuntimeQuestionItem question, RuntimeFileItem? file)
    {
        try
        {
            if (file is not null)
            {
                await OnFileRemoved.InvokeAsync(file);
            }

            _ = _fileNames.Remove(question.Id);
            _validationError = null;
        }
        catch (Exception exception)
        {
            _validationError = exception.Message;
        }
    }

    private async Task Submit()
    {
        if (_isSubmitting)
        {
            return;
        }

        var missingQuestion = Sections.SelectMany(section => section.Questions).FirstOrDefault(question => question.IsRequired && !HasValue(question));
        if (missingQuestion is not null)
        {
            _validationError = $"'{missingQuestion.Label}' is required.";
            return;
        }

        var answers = Sections.SelectMany(section => section.Questions).Where(question => question.Type != QuestionnaireQuestionType.File).Select(question =>
            new QuestionnaireAnswerValue(question.Id, question.Type is QuestionnaireQuestionType.ShortText or QuestionnaireQuestionType.LongText ? _texts.GetValueOrDefault(question.Id) : null, null, null, null, null,
                question.Type == QuestionnaireQuestionType.Address ? JsonSerializer.Serialize(GetAddress(question.Id)) : null, null) with
            {
                IntegerValue = question.Type == QuestionnaireQuestionType.Integer ? _integers.GetValueOrDefault(question.Id) : null,
                DecimalValue = question.Type == QuestionnaireQuestionType.Decimal ? _decimals.GetValueOrDefault(question.Id) : null,
                DateValue = question.Type == QuestionnaireQuestionType.Date ? _dates.GetValueOrDefault(question.Id) : null,
                BooleanValue = question.Type == QuestionnaireQuestionType.Boolean ? _booleans.GetValueOrDefault(question.Id) : null,
                OptionIds = question.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice ? _optionIds.GetValueOrDefault(question.Id)?.ToList() : null
            }).ToList();
        _validationError = null;
        try
        {
            _isSubmitting = true;
            await OnNext.InvokeAsync(answers);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private bool HasValue(RuntimeQuestionItem question)
    {
        return question.Type switch
        {
            QuestionnaireQuestionType.ShortText or QuestionnaireQuestionType.LongText => !string.IsNullOrWhiteSpace(_texts.GetValueOrDefault(question.Id)),
            QuestionnaireQuestionType.Address => IsComplete(GetAddress(question.Id)),
            QuestionnaireQuestionType.Integer => _integers.GetValueOrDefault(question.Id) is not null,
            QuestionnaireQuestionType.Decimal => _decimals.GetValueOrDefault(question.Id) is not null,
            QuestionnaireQuestionType.Date => _dates.GetValueOrDefault(question.Id) is not null,
            QuestionnaireQuestionType.Boolean => _booleans.GetValueOrDefault(question.Id) is not null,
            QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice => _optionIds.GetValueOrDefault(question.Id)?.Count > 0,
            QuestionnaireQuestionType.File => question.Files.Count > 0
                || !string.IsNullOrWhiteSpace(_fileNames.GetValueOrDefault(question.Id)),
            _ => false
        };
    }

    private static bool IsComplete(QuestionnaireAddressRequest address)
    {
        return !string.IsNullOrWhiteSpace(address.Country)
            && !string.IsNullOrWhiteSpace(address.Building)
            && !string.IsNullOrWhiteSpace(address.Street)
            && !string.IsNullOrWhiteSpace(address.Number)
            && !string.IsNullOrWhiteSpace(address.City)
            && !string.IsNullOrWhiteSpace(address.Phone)
            && !string.IsNullOrWhiteSpace(address.Fax)
            && !string.IsNullOrWhiteSpace(address.Email)
            && !string.IsNullOrWhiteSpace(address.Website);
    }

    #endregion
}
