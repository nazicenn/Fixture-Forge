namespace FixtureForge.Application.Schema;

public static class SupportedDataTypes
{
    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        // Integer
        "int", "bigint", "smallint", "tinyint",
        // String
        "nvarchar", "varchar", "char", "nchar", "text", "ntext",
        // Date/Time
        "datetime2", "datetime", "date", "time", "datetimeoffset",
        // Boolean
        "bit",
        // Decimal/Numeric
        "decimal", "numeric", "float", "real", "money", "smallmoney",
        // Other
        "uniqueidentifier", "varbinary", "binary", "image"
    };

    // MaxLength zorunlu olan tipler
    public static readonly HashSet<string> RequireMaxLength = new(StringComparer.OrdinalIgnoreCase)
    {
        "nvarchar", "varchar", "char", "nchar", "varbinary", "binary"
    };

    // Precision/Scale zorunlu olan tipler
    public static readonly HashSet<string> RequirePrecision = new(StringComparer.OrdinalIgnoreCase)
    {
        "decimal", "numeric"
    };
}