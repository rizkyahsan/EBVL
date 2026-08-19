using System.Text.RegularExpressions;

namespace EBVL.Shared.Statics.Common.Extensions;

public static class StringExtensions
{
    public static string SplitWords(this string sentence)
    {
        var pattern = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])";
        var regex = new Regex(pattern, RegexOptions.Compiled);
        var removedExistingSpace = sentence.Replace(" ", string.Empty);

        return regex.Replace(removedExistingSpace, " ");
    }

    public static string ReplaceNewLineToBr(this string text)
    {
        var pattern = @"(\r\n|\r|\n)+";
        var regex = new Regex(pattern, RegexOptions.Compiled);

        return regex.Replace(text, "<br />");
    }
}
