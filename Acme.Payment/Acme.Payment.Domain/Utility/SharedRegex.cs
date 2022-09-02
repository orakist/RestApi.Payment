using System.Text.RegularExpressions;

namespace Acme.Payment.Domain.Utility;

public static class SharedRegex
{
    public static readonly Regex Integer = new(@"^[0-9]+$", RegexOptions.Compiled);
    public static readonly Regex Floating = new(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
    private static readonly Regex LineRegex = new(@"\s+", RegexOptions.Compiled);

    public static string ClearSingleLineText(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return null;

        return LineRegex.Replace(data.Trim(), " ");
    }
}