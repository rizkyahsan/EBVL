using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

public partial class QuestionnaireEditor
{
    [Parameter] public string BusinessProcess { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> BusinessProcessChanged { get; set; }
    [Parameter, EditorRequired] public List<SectionModel> Sections { get; set; } = [];
    [Parameter, EditorRequired] public List<RuleModel> Rules { get; set; } = [];
    [Parameter] public bool ReadOnly { get; set; }

    private List<QuestionModel> Questions => Sections.SelectMany(x => x.Questions).ToList();

    private static void Move<T>(List<T> items, int index, int delta)
    {
        var item = items[index];
        items.RemoveAt(index);
        items.Insert(index + delta, item);
    }

    private void RemoveQuestion(SectionModel section, QuestionModel question)
    {
        _ = section.Questions.Remove(question);
        _ = Rules.RemoveAll(x => x.SourceQuestionId == question.Id || x.TargetQuestionId == question.Id);
    }

    private void AddRule()
    {
        var questions = Questions;
        Rules.Add(new RuleModel
        {
            SourceQuestionId = questions[0].Id,
            TargetQuestionId = questions[1].Id
        });
    }
}
