using System.Security.Claims;
using System.Text;
using Microsoft.JSInterop;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorMonitoring.RegistrationVendors.Components;
using EBVL.Shared.Dto.Modules.Main.VendorMonitoring.GetVendorRegistration;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorMonitoring.RegistrationVendors.Pages;

public partial class Details
{
    [Parameter]
    public Guid VendorRegistrationId { get; set; }

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; init; }

    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private VendorRegistrationDetail? _item;
    private int? _evaluationScore;
    private bool _isSeniorManager;

    private readonly DocumentItem[] _documents = [.. DocumentEvidenceFor.All.Select(document => new DocumentItem(document.Name, $"{document.Key}.pdf"))];

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadItem();

        var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
        _isSeniorManager = user.Claims
            .Where(claim => claim.Type is ClaimTypes.Role or "role")
            .Select(claim => claim.Value)
            .Any(role => role.Contains("Senior Manager", StringComparison.OrdinalIgnoreCase)
                || role.Contains("SeniorManager", StringComparison.OrdinalIgnoreCase)
                || role.Contains("Sr Man", StringComparison.OrdinalIgnoreCase));
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            VendorMonitoringRegistrationVendorsBreadcrumbFor.VendorMonitoring,
            new BreadcrumbItem(VendorMonitoringRegistrationVendorsDisplayTextFor.RegistrationVendor, VendorMonitoringRegistrationVendorsRouteFor.Index),
            CommonBreadcrumbFor.Active(VendorMonitoringRegistrationVendorsDisplayTextFor.DetailRegistrationVendor)
        ];
    }

    private Task LoadItem()
    {
        _item = new VendorRegistrationDetail
        {
            Id = VendorRegistrationId,
            CompanyName = "VENDOR XYZ",
            CompanyEmail = "email@vendor.com",
            PicEmail = "pic@vendor.com",
            CompanyPhoneNumber = "0812345678",
            PicPhoneNumber = "0856234567",
            Website = "https://www.vendor-xyz.com",
            CompanyService = "Produk",
            FactoryAddress = "Jalan Maju Jaya No 88 RT 99 IKN, Sepaku, Kalimantan Timur",
            BrandRepresentative = "Proxima Metalurgi",
            CompanyStatus = "Agen Distributor Tunggal",
            SapVendorNumber = "647122345",
            TaxNumber = "90909090909090"
        };

        return Task.CompletedTask;
    }

    private static string GetAnswer(int questionNumber)
    {
        return $"Jawaban vendor {questionNumber}";
    }

    private static string GetQuestionFileName(int questionNumber)
    {
        return $"Evidence-Question-{questionNumber}.pdf";
    }

    private async Task PreviewDocument(string fileName)
    {
        var parameters = new DialogParameters<DialogDocumentPreview> { { dialog => dialog.FileName, fileName } };
        _ = await DialogService.ShowAsync<DialogDocumentPreview>("Preview Evidence", parameters, new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true });
    }

    private async Task DownloadDocument(string fileName)
    {
        var content = Convert.ToBase64String(Encoding.UTF8.GetBytes($"Mock evidence document: {fileName}"));
        await JSRuntime.InvokeVoidAsync(JavaScriptIdentifierFor.DownloadFile, fileName, "text/plain", content);
    }

    private async Task SubmitEvaluation()
    {
        if (_evaluationScore is null)
        {
            return;
        }

        var passed = _evaluationScore >= 80;
        var title = passed ? "Nilai Evaluasi Terpenuhi" : "Nilai Evaluasi Belum Terpenuhi";
        var message = passed
            ? "Registrasi vendor akan diteruskan ke approval by Sr MAN"
            : "Registrasi vendor gagal, email informasi reject akan dikirimkan oleh sistem";

        _ = await ShowWorkflowDialog(title, message, "Back", false);
        _ = Snackbar.Add(passed ? "Status berubah menjadi Waiting Approval by Sr Man." : "Pengajuan ditolak dan notifikasi email dijadwalkan.", passed ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
    }

    private async Task SubmitApproval()
    {
        var confirmed = await ShowWorkflowDialog("Approve Vendor Registration", "Anda akan melakukan approval registrasi Vendor, apakah anda yakin?", "Ya", true);
        if (confirmed)
        {
            _ = Snackbar.Add("Vendor registration approved by Sr Man.", MudBlazor.Severity.Success);
        }
    }

    private async Task<bool> ShowWorkflowDialog(string title, string message, string confirmText, bool showCancel)
    {
        var parameters = new DialogParameters<DialogWorkflow>
        {
            { dialog => dialog.Title, title },
            { dialog => dialog.Message, message },
            { dialog => dialog.ConfirmText, confirmText },
            { dialog => dialog.ShowCancel, showCancel }
        };
        var dialog = await DialogService.ShowAsync<DialogWorkflow>(string.Empty, parameters, new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true });
        var result = await dialog.Result;
        return result is not null && !result.Canceled;
    }

    private sealed record DocumentItem(string Name, string FileName);
}
