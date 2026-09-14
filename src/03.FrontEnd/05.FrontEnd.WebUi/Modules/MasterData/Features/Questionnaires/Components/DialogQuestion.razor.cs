using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

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
            IsRequired = Model.IsRequired,
            IsVisible = Model.IsVisible,
            IsActive = Model.IsActive,
            AnswerRule = Model.AnswerRule,
            Order = Model.Order,
            Options = Model.Options
                .Select(x => new OptionModel { Code = x.Code, Label = x.Label })
                .ToList()
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
                return;
            }

            _model.Code = string.IsNullOrWhiteSpace(_model.Code) ? Code(_model.Label) : _model.Code;
            _model.IsRequired = _model.AnswerRule == QuestionnaireAnswerRule.Mandatory;
            _model.IsVisible = true;

            if (Model is null)
            {
                var request = new AddQuestionnaireQuestionRequest
                {
                    Name = _model.Label,
                    Code = _model.Code,
                    Description = _model.Hint,
                    AnswerType = _model.Type,
                    AnswerRule = _model.AnswerRule,
                    Order = _model.Order,
                    IsActive = _model.IsActive
                };
                var command = new AddQuestionnaireQuestionCommand(QuestionnaireId, SectionId, request);
                var response = await Sender.Send(command);
                Dialog.Close(DialogResult.Ok(response.Item));
            }
            else
            {
                await OnSubmit.InvokeAsync(_model);
                Dialog.Close();
            }
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private static string Code(string value)
    {
        return string.Join(
            '_',
            value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }
}
