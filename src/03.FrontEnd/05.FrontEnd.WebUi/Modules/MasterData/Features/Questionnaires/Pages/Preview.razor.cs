using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Pages;

public partial class Preview
{
    [Parameter] public Guid Id { get; set; }

    private QuestionnaireItem? _item;

    protected override async Task OnParametersSetAsync()
    {
        LoadBreadcrumbs();

        try
        {
            _isLoading = true;
            _item = (await Sender.Send(new GetQuestionnaireQuery(Id))).Item;
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

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            new("Questionnaire", QuestionnaireRouteFor.Index),
            CommonBreadcrumbFor.Active("Preview")
        ];
    }

    private static string Marker(QuestionnaireAnswerRule rule)
    {
        return rule switch
        {
            QuestionnaireAnswerRule.Mandatory => "*",
            QuestionnaireAnswerRule.AddedValue => "**",
            QuestionnaireAnswerRule.Optional => string.Empty,
            _ => string.Empty
        };
    }
}
