using EBVL.FrontEnd.WebUi.Layouts.Models;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Pages;

public partial class Landing
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [CascadingParameter]
    public DisplayInfo DisplayInfo { get; set; } = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    private int _activeSlide;
    private string? _sapVendorNumber;
    private string? _trackingError;
    private string? _trackingResult;

    private readonly HeroSlide[] _slides =
    [
        new("Connect your brand to Indonesia's energy future", "Register and manage your brand vendor information through one trusted digital platform.", "Join Us", "/Vendor-Registration"),
        new("Transparent registration, clearer progress", "Complete the questionnaire, submit evidence, and follow every verification stage online.", "Track Registration", "#track"),
        new("Stronger partnerships start here", "Build a verified vendor profile and unlock opportunities across Pertamina Patra Niaga.", "Learn More", "#opportunity-title")
    ];

    private readonly OpportunityItem[] _opportunities =
    [
        new(Icons.Material.Filled.AppRegistration, "Simple Registration", "A guided registration flow helps vendors submit company, brand, questionnaire, and evidence data accurately.", "Start registration", "/Vendor-Registration"),
        new(Icons.Material.Filled.FactCheck, "Transparent Verification", "Monitor evaluation and approval progress with clear statuses from initial submission through final verification.", "Track your status", "#track"),
        new(Icons.Material.Filled.Handshake, "Trusted Collaboration", "A verified electronic brand vendor list supports reliable sourcing and long-term business collaboration.", "Explore e-BVL", "#faq")
    ];

    private readonly FaqItem[] _faqs =
    [
        new("Who can register in e-BVL?", "Companies that own, represent, distribute, or supply brands for Pertamina Patra Niaga opportunities can submit a registration."),
        new("What information should be prepared?", "Prepare company and PIC information, SAP vendor number, brand information, questionnaire responses, and the required supporting evidence in PDF format."),
        new("How does the evaluation process work?", "An administrator reviews the registration and assigns an evaluation score. A score of at least 80 proceeds to Senior Manager approval; lower scores are rejected."),
        new("How can I check my registration status?", "Use the SAP vendor number entered during registration in the tracking section. Persisted status information will be provided after backend integration."),
        new("Who should I contact for assistance?", "Contact Pertamina Call Center 135 through Main Menu 2, option 4, or email cp@pertamina.com.")
    ];

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthenticationStateTask).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            NavigationManager.NavigateTo(MainRouteFor.Index, true);
        }
    }

    private void PreviousSlide()
    {
        _activeSlide = (_activeSlide - 1 + _slides.Length) % _slides.Length;
    }

    private void NextSlide()
    {
        _activeSlide = (_activeSlide + 1) % _slides.Length;
    }

    private void SelectSlide(int index)
    {
        _activeSlide = index;
    }

    private void TrackRegistration()
    {
        _trackingError = null;
        _trackingResult = null;

        if (string.IsNullOrWhiteSpace(_sapVendorNumber))
        {
            _trackingError = "SAP vendor number is required.";
            return;
        }

        if (!_sapVendorNumber.All(char.IsDigit))
        {
            _trackingError = "SAP vendor number must contain numbers only.";
            return;
        }

        _trackingResult = $"Registration {_sapVendorNumber.Trim()} is currently waiting for administrator evaluation.";
    }

    private sealed record HeroSlide(string Title, string Description, string ActionText, string ActionUrl);
    private sealed record OpportunityItem(string Icon, string Title, string Description, string ActionText, string ActionUrl);
    private sealed record FaqItem(string Question, string Answer);
}
