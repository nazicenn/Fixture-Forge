using System.Text.RegularExpressions;

namespace FixtureForge.Infrastructure.Queries;

public static class QueryMasker
{
    // 'metin' → '***'
    private static readonly Regex StringLiteralRegex = new(@"N?'([^']|'')*'", RegexOptions.Compiled);

    // 123, 45.67 → ?
    private static readonly Regex NumericLiteralRegex = new(@"\b\d+(\.\d+)?\b", RegexOptions.Compiled);

    // -- yorum
    private static readonly Regex LineCommentRegex = new(@"--[^\r\n]*", RegexOptions.Compiled);

    // /* yorum */
    private static readonly Regex BlockCommentRegex = new(@"/\*.*?\*/", RegexOptions.Compiled | RegexOptions.Singleline);

    public static string Mask(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return string.Empty;

        var masked = BlockCommentRegex.Replace(query, "/*...*/");
        masked = LineCommentRegex.Replace(masked, "--...");
        masked = StringLiteralRegex.Replace(masked, "'***'");
        masked = NumericLiteralRegex.Replace(masked, "?");
        return masked;
    }
}