using EBVL.FrontEnd.Logics.Modules.Examples.Emails.SendEmailWithTemplate;
using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmailWithTemplate;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Emails.Pages;

public partial class SendEmailWithTemplate
{
    private const long MaximumFileSize = 20_971_520;
    private MudForm _form = default!;
    private SendEmailWithTemplateModel _model = default!;
    private SendEmailWithTemplateModelValidator _validator = default!;

    protected override void OnInitialized()
    {
        _pageTitle = ExamplesEmailsDisplayTextFor.SendEmailWithTemplate;

        LoadBreadcrumbs();

        _model = new()
        {
            Tos = "someone1@pertamina.com, someone2@pertamina.com",
            Ccs = "someone.else1@pertamina.com, someone.else2@pertamina.com",
            Bccs = "another.person1@pertamina.com, another.person2@pertamina.com",
            ItemsCount = 3,
            Attachments = []
        };

        _validator = new();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            ExamplesEmailsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesEmailsDisplayTextFor.SendEmailWithTemplate)
        ];
    }

    private void FilesUpdated(IList<IBrowserFile> files)
    {
        _model.Attachments = files;
    }

    private async Task Send()
    {
        try
        {
            _isLoading = true;

            ClearException();

            await _form.RunValidation();

            var addressSeparator = ", ";

            var tos = _model.Tos.Split(addressSeparator)
                .Select(x => new EmailContactItem
                {
                    Name = x,
                    Address = x
                }).ToList();

            var ccs = new List<EmailContactItem>();

            if (!string.IsNullOrWhiteSpace(_model.Ccs))
            {
                ccs.AddRange(_model.Ccs.Split(addressSeparator)
                    .Select(x => new EmailContactItem
                    {
                        Name = x,
                        Address = x
                    }));
            }

            var bccs = new List<EmailContactItem>();

            if (!string.IsNullOrWhiteSpace(_model.Bccs))
            {
                bccs.AddRange(_model.Bccs.Split(addressSeparator)
                    .Select(x => new EmailContactItem
                    {
                        Name = x,
                        Address = x
                    }));
            }

            var attachments = new List<EmailAttachmentItem>();

            foreach (var file in _model.Attachments)
            {
                attachments.Add(new EmailAttachmentItem
                {
                    FileName = file.Name,
                    ContentType = file.ContentType,
                    FileContent = await file.ToBytesAsync(MaximumFileSize)
                });
            }

            var command = new SendEmailWithTemplateCommand
            {
                Tos = tos,
                Ccs = ccs,
                Bccs = bccs,
                ItemsCount = _model.ItemsCount,
                Attachments = attachments
            };

            var response = await Sender.Send(command);

            Snackbar.AddSuccess(response.Item.Message);
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

public sealed record SendEmailWithTemplateModel
{
    [Label(nameof(Tos))]
    public required string Tos { get; set; }

    [Label(nameof(Ccs))]
    public required string? Ccs { get; set; }

    [Label(nameof(Bccs))]
    public required string? Bccs { get; set; }

    [Label("Items Count")]
    public required int ItemsCount { get; set; }

    [Label(nameof(Attachments))]
    public required IList<IBrowserFile> Attachments { get; set; }
}

public sealed class SendEmailWithTemplateModelValidator : AbstractValidatorBase<SendEmailWithTemplateModel>
{
    public SendEmailWithTemplateModelValidator()
    {
        _ = RuleFor(x => x.Tos).NotEmpty();
        _ = RuleFor(x => x.ItemsCount).InclusiveBetween(2, 5);
    }
}
