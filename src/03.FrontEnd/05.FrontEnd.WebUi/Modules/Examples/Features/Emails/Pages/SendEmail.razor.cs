using EBVL.FrontEnd.Logics.Modules.Examples.Emails.SendEmail;
using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Emails.Pages;

public partial class SendEmail
{
    private const long MaximumFileSize = 20_971_520;
    private MudForm _form = default!;
    private SendEmailModel _model = default!;
    private SendEmailModelValidator _validator = default!;

    protected override void OnInitialized()
    {
        _pageTitle = ExamplesEmailsDisplayTextFor.SendEmail;

        LoadBreadcrumbs();

        var nowDisplayText = DateTimeOffset.Now.ToDisplayText(DateTimeFormatFor.LongDateTime);

        _model = new()
        {
            Tos = "someone1@pertamina.com, someone2@pertamina.com",
            Ccs = "someone.else1@pertamina.com, someone.else2@pertamina.com",
            Bccs = "another.person1@pertamina.com, another.person2@pertamina.com",
            Subject = $"[{AppConfigFrontEndOptions.Value.AppNickName}] Testing Email {nowDisplayText}",
            Body = $"<p>Hi, this is a Testing Email from {AppConfigFrontEndOptions.Value.AppNickName}.</p>\n" +
            $"<p>Current date and time: <b>{nowDisplayText}</b>.</p>",
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
            CommonBreadcrumbFor.Active(ExamplesEmailsDisplayTextFor.SendEmail)
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

            var command = new SendEmailCommand
            {
                Tos = tos,
                Ccs = ccs,
                Bccs = bccs,
                Subject = _model.Subject,
                Body = _model.Body,
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

public sealed record SendEmailModel
{
    [Label(nameof(Tos))]
    public required string Tos { get; set; }

    [Label(nameof(Ccs))]
    public required string? Ccs { get; set; }

    [Label(nameof(Bccs))]
    public required string? Bccs { get; set; }

    [Label(nameof(Subject))]
    public required string Subject { get; set; }

    [Label(nameof(Body))]
    public required string Body { get; set; }

    [Label(nameof(Attachments))]
    public required IList<IBrowserFile> Attachments { get; set; }
}

public sealed class SendEmailModelValidator : AbstractValidatorBase<SendEmailModel>
{
    public SendEmailModelValidator()
    {
        _ = RuleFor(x => x.Tos).NotEmpty();
        _ = RuleFor(x => x.Subject).NotEmpty();
        _ = RuleFor(x => x.Body).NotEmpty();
    }
}
