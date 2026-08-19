using EBVL.BackEnd.Services.EmailBlast2.Model;

namespace EBVL.BackEnd.Logics.Common.Builder;

public static class EmailInputBuilder
{
    /// <summary>
    /// Builds a SendEmailInput2 by replacing named tokens (e.g., {DisplayName}) in the template.
    /// </summary>
    public static async Task<SendEmailInput2> BuildTheTemplate(
        IDatabaseService db,
        string emailWith,
        string module,
        string action,
        Dictionary<string, string> tokenDict,
        string? subject = null,
        string? defaultFrom = null,
        IEnumerable<EmailContact2>? explicitTos = null,
        IEnumerable<EmailContact2>? explicitCCs = null,
        IEnumerable<EmailAttachment2>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        var template = await FetchTemplateAsync(db, module, action, cancellationToken);
        var formattedBody = ReplaceBodyPlaceholders(template.Content, tokenDict);

        return AssembleEmailInput(template, emailWith, formattedBody, subject, defaultFrom, explicitTos, explicitCCs, attachments);
    }

    // --- Private Core Engines ---

    private static async Task<EmailTemplate> FetchTemplateAsync(IDatabaseService db, string module, string action, CancellationToken cancellationToken)
    {
        return await db.EmailTemplates
            .Where(x => !x.IsDeleted && x.Module == module && x.Action == action)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");
    }

    private static string ReplaceBodyPlaceholders(string templateContent, Dictionary<string, string> tokenDict)
    {
        var formattedBody = templateContent;

        foreach (var kvp in tokenDict)
        {
            formattedBody = formattedBody.Replace($"{{{kvp.Key}}}", kvp.Value);
        }

        return formattedBody;
    }

    private static SendEmailInput2 AssembleEmailInput(EmailTemplate template,
        string emailWith,
        string body,
        string? subject = null,
        string? defaultFrom = null,
        IEnumerable<EmailContact2>? explicitTos = null,
        IEnumerable<EmailContact2>? explicitCCs = null,
        IEnumerable<EmailAttachment2>? attachments = null)
    {
        var emailInput = new SendEmailInput2
        {
            Module = template.Module,
            Action = template.Action,
            EmailWith = emailWith,
            Tos = explicitTos?.ToList() ?? ParseEmails(template.DefaultTo),
            Ccs = explicitCCs?.ToList() ?? ParseEmails(template.DefaultCc),
            Bccs = [], // Default to empty list as per record structure
            Subject = !string.IsNullOrEmpty(subject) ? subject : template.Subject,
            Body = body,
            Attachments = attachments?.ToList() ?? []
        };

        // Only assign From if defaultFrom is provided
        if (!string.IsNullOrWhiteSpace(defaultFrom))
        {
            emailInput.DefaultFrom = defaultFrom;
        }

        return emailInput;
    }

    private static List<EmailContact2> ParseEmails(string emailString)
    {
        if (string.IsNullOrWhiteSpace(emailString))
        {
            return [];
        }

        return [.. emailString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(email => new EmailContact2 { Address = email, Name = email })];
    }
}
