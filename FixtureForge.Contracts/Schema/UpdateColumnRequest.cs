namespace FixtureForge.Contracts.Schema;

public record UpdateColumnRequest(
    string Name,
    string DataType,
    bool IsNullable,
    bool IsPrimaryKey,
    bool IsUnique,
    string? DefaultValue,
    int? MaxLength,
    int? Precision,
    int? Scale,
    int OrdinalPosition);