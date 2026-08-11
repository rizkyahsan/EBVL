using EBVL.FrontEnd.Logics.Modules.Administration.Audits.GetAudit;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Audits.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private AuditItem _audit = default!;

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = AuditsDisplayTextFor.Audit;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            AdministrationBreadcrumbFor.Index,
            AdministrationAuditsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetAuditQuery
            {
                AuditId = Id
            };

            var response = await Sender.Send(query);

            _audit = response.Item;
            _pageTitle = $"{_audit.ActionType.GetDisplayText()} {_audit.EntityName}";

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));
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
}
