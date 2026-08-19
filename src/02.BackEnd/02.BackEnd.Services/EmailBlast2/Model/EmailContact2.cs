using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EBVL.BackEnd.Services.EmailBlast2.Model;

public sealed record EmailContact2
{
    public required string Name { get; init; }

    public required string Address { get; init; }

    [CompilerGenerated]
    [SetsRequiredMembers]
    private EmailContact2(EmailContact2 original)
    {
        Name = original.Name;
        Address = original.Address;
    }

    public EmailContact2()
    {
    }
}
