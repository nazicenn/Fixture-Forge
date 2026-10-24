namespace FixtureForge.Infrastructure.Queries;

public static class QueryClassifier
{
    public static string Classify(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return "OTHER";
        var trimmed = query.TrimStart();
        // İlk kelimeyi al
        var firstWord = trimmed.Split(new[] { ' ', '\t', '\r', '\n', '(' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToUpperInvariant() ?? "OTHER";
        return firstWord switch
        {
            "SELECT" => "SELECT",
            "INSERT" => "INSERT",
            "UPDATE" => "UPDATE",
            "DELETE" => "DELETE",
            "CREATE" or "ALTER" or "DROP" or "TRUNCATE" => "DDL",
            _ => "OTHER"
        };
    }
}