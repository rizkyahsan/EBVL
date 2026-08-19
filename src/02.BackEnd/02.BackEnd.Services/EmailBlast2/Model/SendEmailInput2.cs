using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EBVL.BackEnd.Services.EmailBlast2.Model;

public sealed record SendEmailInput2
{
    public required string EmailWith { get; set; }

    public required string Module { get; set; }

    public required string Action { get; set; }

    public string? DefaultFrom { get; set; }

    public required IList<EmailContact2> Tos { get; init; }

    public IList<EmailContact2> Ccs { get; init; } = [];

    public IList<EmailContact2> Bccs { get; init; } = [];

    public required string Subject { get; init; }

    public required string Body { get; init; }

    public IList<EmailAttachment2> Attachments { get; init; } = [];

    [CompilerGenerated]
    [SetsRequiredMembers]
    private SendEmailInput2(SendEmailInput2 original)
    {
        EmailWith = original.EmailWith;
        Module = original.Module;
        Action = original.Action;
        Tos = original.Tos;
        Ccs = original.Ccs;
        Bccs = original.Bccs;
        Subject = original.Subject;
        Body = original.Body;
        Attachments = original.Attachments;
    }

    public SendEmailInput2() { }
}
