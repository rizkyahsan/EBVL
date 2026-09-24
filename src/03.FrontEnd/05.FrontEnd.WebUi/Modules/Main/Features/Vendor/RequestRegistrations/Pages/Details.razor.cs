using System.Text;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Components;
using EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistration;
using EBVL.Shared.Statics.VendorRegistrations;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Pages;

public partial class Details
{
    [Parameter]
    public Guid RequestRegistrationId { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private RequestRegistrationDetail? _item;
    private string? _approvalNotes;
    private bool _approvalNotesError;
    private bool _showValidationErrors;
    private ReviewItem[] _companyFields = [];
    private readonly DocumentItem[] _documents = [.. DocumentEvidenceFor.All.Select(document => new DocumentItem(document.Name, $"{document.Key}.pdf"))];
    private readonly QuestionItem[] _questionFields = [.. QuestionnaireFor.All.Select(question => new QuestionItem(question.Section, question.Number, question.Label, question.IsRequired, question.IsFileUpload, GetAnswer(question.Number), GetQuestionFileName(question.Number)))];
    private readonly ActivityItem[] _activities =
    [
        new("Registration submitted", "Vendor completed and submitted the registration form.", "24 September 2026, 09:35"),
        new("Review assigned", "Request registration assigned to Admin MSAir.", "24 September 2026, 10:02")
    ];

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            VendorRequestRegistrationsBreadcrumbFor.Vendor,
            new BreadcrumbItem(VendorRequestRegistrationsDisplayTextFor.RequestRegistration, VendorRequestRegistrationsRouteFor.Index),
            CommonBreadcrumbFor.Active(VendorRequestRegistrationsDisplayTextFor.DetailRequestRegistration)
        ];
    }

    private Task LoadItem()
    {
        _item = new RequestRegistrationDetail
        {
            Id = RequestRegistrationId,
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
        _companyFields =
        [
            new("Company Name", _item.CompanyName),
            new("Company Email", _item.CompanyEmail),
            new("PIC Email", _item.PicEmail),
            new("Company Phone Number", _item.CompanyPhoneNumber),
            new("PIC Phone Number", _item.PicPhoneNumber),
            new("Company Website", _item.Website, true),
            new("Company Service", _item.CompanyService),
            new("Factory Address", _item.FactoryAddress),
            new("Brand Owner / Representative", _item.BrandRepresentative),
            new("Company Status", _item.CompanyStatus),
            new("SAP Vendor Number", _item.SapVendorNumber),
            new("NPWP Vendor Number", _item.TaxNumber)
        ];
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

    private void GoBack()
    {
        NavigationManager.NavigateTo(VendorRequestRegistrationsRouteFor.Index);
    }

    private Task RejectRegistration()
    {
        return SubmitDecision(false);
    }

    private Task ApproveRegistration()
    {
        return SubmitDecision(true);
    }

    private void SetApprovalNotes(string? value)
    {
        _approvalNotes = value;
        _approvalNotesError = false;
    }

    private async Task SubmitDecision(bool isApproved)
    {
        _showValidationErrors = true;
        _approvalNotesError = !isApproved && string.IsNullOrWhiteSpace(_approvalNotes);
        if (!IsReviewValid())
        {
            _ = Snackbar.Add("Lengkapi penilaian setiap item dan isi remark untuk nilai Invalid.", MudBlazor.Severity.Error);
            return;
        }

        if (_approvalNotesError)
        {
            _ = Snackbar.Add("Approval Notes wajib diisi untuk Reject.", MudBlazor.Severity.Error);
            return;
        }

        var action = isApproved ? "Approve" : "Reject";
        var confirmed = await ShowWorkflowDialog($"{action} Request Registration", $"Anda akan melakukan {action.ToLowerInvariant()} registrasi vendor. Lanjutkan?", action, true);
        if (confirmed)
        {
            _ = Snackbar.Add($"Request registration berhasil di-{action.ToLowerInvariant()}.", isApproved ? MudBlazor.Severity.Success : MudBlazor.Severity.Error);
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

    private bool IsReviewValid()
    {
        return _companyFields.Cast<ReviewItem>()
            .Concat(_documents)
            .Concat(_questionFields)
            .All(item => item.IsValid is not null && (item.IsValid is true || !string.IsNullOrWhiteSpace(item.Remark)));
    }

    private static void SetValidity(ReviewItem item, bool? value)
    {
        item.IsValid = value;
        if (value is not false)
        {
            item.Remark = null;
        }
    }

    private static void SetRemark(ReviewItem item, string? value)
    {
        item.Remark = value;
    }

    private class ReviewItem(string label, string value, bool isLink = false)
    {
        public string Label { get; } = label;
        public string Value { get; } = value;
        public bool IsLink { get; } = isLink;
        public bool? IsValid { get; set; }
        public string? Remark { get; set; }
    }

    private sealed class DocumentItem(string name, string fileName) : ReviewItem(name, fileName)
    {
        public string Name { get; } = name;
        public string FileName { get; } = fileName;
    }

    private sealed class QuestionItem(string section, int number, string label, bool isRequired, bool isFileUpload, string value, string fileName) : ReviewItem(label, value)
    {
        public string Section { get; } = section;
        public int Number { get; } = number;
        public bool IsRequired { get; } = isRequired;
        public bool IsFileUpload { get; } = isFileUpload;
        public string FileName { get; } = fileName;
        public string DisplayLabel => $"{Number}. {(IsRequired ? "*" : string.Empty)} {Label}";
    }

    private sealed record ActivityItem(string Title, string Description, string OccurredAt);
}
