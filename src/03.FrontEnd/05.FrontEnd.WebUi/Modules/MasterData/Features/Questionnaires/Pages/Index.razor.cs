using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Pages;

public partial class Index
{
    private List<QuestionnaireListItem> _items = [];
    private string? _search;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await Load();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active("Questionnaire")
        ];
    }

    private async Task Load()
    {
        try
        {
            _isLoading = true;
            ClearException();

            var response = await Sender.Send(new GetQuestionnairesQuery());
            _items = response.Items.ToList();
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

    private bool Filter(QuestionnaireListItem item)
    {
        return string.IsNullOrWhiteSpace(_search)
            || $"{item.BusinessProcess} {item.Section} {Vendor(item.VendorType)}".Contains(_search, StringComparison.OrdinalIgnoreCase);
    }

    private int Number(QuestionnaireListItem item)
    {
        return _items.IndexOf(item) + 1;
    }

    private static string Vendor(VendorCompanyStatusType? value)
    {
        return value switch
        {
            null => "All",
            VendorCompanyStatusType.Manufacture => "Vendor",
            VendorCompanyStatusType.SoleDistributorAgent => "Sole Agent",
            VendorCompanyStatusType.AuthorizedAgent => "Representative Office",
            _ => "Representative Office"
        };
    }

    private async Task ShowAdd()
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        var dialog = await DialogService.ShowAsync<DialogAdd>("Add New Questionnaire Section", options: options);
        var result = await dialog.Result;

        if (result is not { Canceled: false, Data: QuestionnaireItem created })
        {
            return;
        }

        _search = null;
        _items = created.Sections
            .Select(section => new QuestionnaireListItem(created.QuestionnaireId, section.Id, created.Code, created.BusinessProcess, section.CompanyType, section.Title, created.IsActive, section.IsActive))
            .Concat(_items.Where(item => item.QuestionnaireId != created.QuestionnaireId))
            .OrderBy(item => item.BusinessProcess)
            .ThenBy(item => item.Section)
            .ToList();

        await InvokeAsync(StateHasChanged);
        await Load();
    }

    private async Task EditSection(QuestionnaireListItem row)
    {
        var questionnaire = (await Sender.Send(new GetQuestionnaireQuery(row.QuestionnaireId))).Item;
        var section = questionnaire.Sections.Single(item => item.Id == row.SectionId);
        var model = new SectionModel
        {
            Id = section.Id,
            BusinessProcess = questionnaire.BusinessProcess,
            Code = section.Code,
            Title = section.Title,
            CompanyType = section.CompanyType,
            IsActive = section.IsActive
        };
        var parameters = new DialogParameters<DialogSection>
        {
            { component => component.Model, model }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        var dialog = await DialogService.ShowAsync<DialogSection>("Edit Questionnaire Section", parameters, options);
        var result = await dialog.Result;

        if (result is null || result.Canceled || result.Data is not SectionModel changed)
        {
            return;
        }

        var request = new AddQuestionnaireRequest
        {
            BusinessProcess = changed.BusinessProcess,
            VendorType = changed.CompanyType,
            Section = changed.Title,
            IsActive = changed.IsActive
        };
        _ = await Sender.Send(new UpdateQuestionnaireSectionCommand(row.QuestionnaireId, row.SectionId, request));

        Snackbar.AddSuccess("Questionnaire section updated.");
        await Load();
    }
}
