namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class ErrorHandling
{
    private const int MaximumCount = 5;
    private static readonly string _errorMessage = $"Counter exceeded the limit of {MaximumCount} clicks.";
    private int _currentCountWithErrorViewer = 0;
    private int _currentCountWithErrorViewerAndSnackbar = 0;
    private int _currentCountWithoutErrorViewer = 0;
    private Exception? _anotherException;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.ErrorHandling)
        ];
    }

    private void Reset()
    {
        _currentCountWithErrorViewer = 0;
        _currentCountWithoutErrorViewer = 0;
        _currentCountWithErrorViewerAndSnackbar = 0;

        _exception = null;
        _anotherException = null;
    }

    private void IncrementCountWithErrorViewer()
    {
        try
        {
            _currentCountWithErrorViewer++;

            if (_currentCountWithErrorViewer > MaximumCount)
            {
                throw new InvalidOperationException(_errorMessage);
            }
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
    }

    private void IncrementCountWithErrorViewerAndSnackbar()
    {
        try
        {
            _currentCountWithErrorViewerAndSnackbar++;

            if (_currentCountWithErrorViewerAndSnackbar > MaximumCount)
            {
                throw new InvalidOperationException(_errorMessage);
            }
        }
        catch (Exception exception)
        {
            _anotherException = exception;
        }
    }

    private void IncrementCountWithoutErrorViewer()
    {
        _currentCountWithoutErrorViewer++;

        if (_currentCountWithoutErrorViewer > MaximumCount)
        {
            throw new InvalidOperationException(_errorMessage);
        }
    }
}
