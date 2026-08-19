using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EBVL.BackEnd.Services.EmailBlast2.Model;

public sealed record EmailAttachment2
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }

    [CompilerGenerated]
    [SetsRequiredMembers]
    private EmailAttachment2(EmailAttachment2 original)
    {
        FileName = original.FileName;
        ContentType = original.ContentType;
        Content = original.Content;
    }

    public EmailAttachment2()
    {
    }
}
