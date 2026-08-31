using EBVL.Shared.Dto.Modules.Main.VendorMonitoring.GetVendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorMonitoring.RegistrationVendors.Pages;

public partial class Index
{
    private IEnumerable<VendorRegistrationItem> _items = [];
    private string? _searchKeyword;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadItems();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            VendorMonitoringRegistrationVendorsBreadcrumbFor.VendorMonitoring,
            CommonBreadcrumbFor.Active(VendorMonitoringRegistrationVendorsDisplayTextFor.RegistrationVendor)
        ];
    }

    private Task LoadItems()
    {
        try
        {
            _isLoading = true;
            ClearException();

            _items =
            [
                CreateItem(1, "PT Technocraft Aji Mumpung", "Nama PIC Vendor A", "Brand A", "Produk & Jasa", "Waiting Approval by Sr Man"),
                CreateItem(2, "Vendor XYZ", "Nama PIC Vendor B", "Brand B", "Produk", "Waiting Evaluation by Adm"),
                CreateItem(3, "PT ABC Indonesia", "Nama PIC Vendor C", "Brand G", "Jasa", "Approved by Sr Man")
            ];
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }

        return Task.CompletedTask;
    }

    private bool FilterItems(VendorRegistrationItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        return item.VendorName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase)
            || item.SubmittedBy.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase)
            || item.Brand.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase)
            || item.ServiceType.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase)
            || item.Status.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase);
    }

    private void ShowDetails(Guid vendorRegistrationId)
    {
        NavigationManager.NavigateTo(VendorMonitoringRegistrationVendorsRouteFor.Details(vendorRegistrationId));
    }

    private static VendorRegistrationItem CreateItem(int number, string vendorName, string submittedBy, string brand, string serviceType, string status)
    {
        return new VendorRegistrationItem
        {
            Id = Guid.NewGuid(),
            Number = number,
            VendorName = vendorName,
            SubmittedBy = submittedBy,
            Brand = brand,
            ServiceType = serviceType,
            Status = status
        };
    }
}
