namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class PageBase : CommonComponentBase
{
    protected string _pageTitle = string.Empty;
    protected List<BreadcrumbItem> _breadcrumbItems = [];

    protected abstract void LoadBreadcrumbs();
}
