using System.Security.Cryptography;
using System.Text;

namespace FixtureForge.Infrastructure.Queries;

public static class QueryFingerprint
{
    public const string PolicyVersion = "v1";

    public static string Compute(string maskedQuery)
    {
        var normalized = Normalize(maskedQuery);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string Normalize(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return string.Empty;

        // Anahtar kelimeleri büyük harfe, boşlukları tek boşluğa
        var tokens = query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        foreach (var token in tokens)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(token.ToUpperInvariant());
        }
        return sb.ToString();
    }
}