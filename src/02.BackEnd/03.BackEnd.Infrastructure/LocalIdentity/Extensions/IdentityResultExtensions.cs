using System.Text;
using Microsoft.AspNetCore.Identity;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity.Extensions;

public static class IdentityResultExtensions
{
    public static string GetErrorSummary(this IdentityResult identityResult)
    {
        var stringBuilderDescription = new StringBuilder();

        for (var i = 0; i < identityResult.Errors.Count(); i++)
        {
            _ = stringBuilderDescription.Append(identityResult.Errors.ElementAt(i).Description);

            if (i < (identityResult.Errors.Count() - 1))
            {
                _ = stringBuilderDescription.Append(Environment.NewLine);
            }
        }

        return stringBuilderDescription.ToString();
    }
}
