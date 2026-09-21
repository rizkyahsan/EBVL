using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

#pragma warning disable IDE0044
public partial class DialogQuestion
{
    [Parameter] public QuestionModel? Model { get; set; }
    [Parameter, EditorRequired] public Guid QuestionnaireId { get; set; }
    [Parameter, EditorRequired] public Guid SectionId { get; set; }
    [Parameter] public int NextOrder { get; set; } = 1;
    [Parameter, EditorRequired] public required EventCallback<QuestionModel> OnSubmit { get; set; }

    private MudForm _form = default!;
    private QuestionModel _model = new();

    protected override void OnInitialized()
    {
        if (Model is null)
        {
            _model.Order = NextOrder;
            return;
        }

        _model = new QuestionModel
        {
            Id = Model.Id,
            Code = Model.Code,
            Label = Model.Label,
            Hint = Model.Hint,
            Placeholder = Model.Placeholder,
            Type = Model.Type,
            CompanyType = Model.CompanyType,
            IsRequired = Model.IsRequired,
            IsVisible = Model.IsVisible,
            IsActive = Model.IsActive,
            AnswerRule = Model.AnswerRule,
            Order = Model.Order,
            Options = [.. Model.Options.Select(x => new OptionModel { Code = x.Code, Label = x.Label })]
        };
    }

    private async Task Submit()
    {
        if (_isLoading)
        {
            return;
        }

        try
        {
            _isLoading = true;
            ClearException();
            await _form.Validate();

            if (!_form.IsValid)
            {
                _isLoading = false;
                return;
            }

            _model.Code = string.IsNullOrWhiteSpace(_model.Code) ? Code(_model.Label) : _model.Code;
            _model.IsRequired = _model.AnswerRule == QuestionnaireAnswerRule.Mandatory;

            if (Model is null)
            {
                await OnSubmit.InvokeAsync(_model);
                _isLoading = false;
                Dialog.Close(DialogResult.Ok(_model));
            }
            else
            {
                if (HasNoChanges())
                {
                    Dialog.Cancel();
                    return;
                }

                await OnSubmit.InvokeAsync(_model);
                _isLoading = false;
                Dialog.Close(DialogResult.Ok(_model));
            }
        }
        catch (Exception exception)
        {
            _exception = exception;
            _isLoading = false;
        }
    }

    private static string Code(string value)
    {
        return string.Join(
            '_',
            value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }

    private bool HasNoChanges()
    {
        return Model is not null
            && Model.Label == _model.Label
            && Model.Hint == _model.Hint
            && Model.Placeholder == _model.Placeholder
            && Model.Type == _model.Type
            && Model.CompanyType == _model.CompanyType
            && Model.IsVisible == _model.IsVisible
            && Model.AnswerRule == _model.AnswerRule
            && Model.Order == _model.Order
            && Model.IsActive == _model.IsActive
            && Model.Options.Select(x => (x.Code, x.Label)).SequenceEqual(_model.Options.Select(x => (x.Code, x.Label)));
    }

    private void AddOption()
    {
        _model.Options.Add(new OptionModel());
    }

    private void RemoveOption(OptionModel option)
    {
        _ = _model.Options.Remove(option);
    }
}
